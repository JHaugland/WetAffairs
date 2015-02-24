using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Timer = System.Timers.Timer;

namespace MemoryMapping
{
    public class MappedArray<TValue> : IDisposable, IEnumerable<TValue>
       where TValue : struct
    {
        #region Private fields
        private string _path;
        private string _fileName;
        private string _uniqueName = "mmf-" + Guid.NewGuid();
        private long _fileSize;
        private MemoryMappedFile _map;
        private int _dataSize;
        private bool _deleteFile = true;
        private byte[] _buffer;
        private IntPtr _memPtr;
        private bool _autogrow = true;

        private Dictionary<int, MapViewStream> _inUse = new Dictionary<int, MapViewStream>(10);
        private Dictionary<int, DateTime> _lastUsedThread = new Dictionary<int, DateTime>();
        private readonly object _lockObject = new object();
        private Timer _pooltimer;
        private bool _isDisposed;
        #endregion

        #region Properties
        /// <summary>
        /// The unique name of the file stored on disk
        /// </summary>
        public string UniqueName
        {
            get { return _uniqueName; }
            set { _uniqueName = value; }
        }

        /// <summary>
        /// Return the number of elements in the array
        /// </summary>
        public long Length
        {
            get
            {
                return _fileSize / _dataSize;
            }
        }

        /// <summary>
        /// Set the position before setting or getting data
        /// </summary>
        public long Position
        {
            set
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;
                _lastUsedThread[threadId] = DateTime.UtcNow;

                Stream s = GetView(threadId);
                s.Position = value * _dataSize;
            }
        }

        /// <summary>
        /// Allow array to automatically grow if you access an indexer larger than the starting size
        /// </summary>
        public bool AutoGrow
        {
            get { return _autogrow; }
            set { _autogrow = value; }
        }

        public override string ToString()
        {
            return string.Format("Length {0}", Length);
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new memory mapped array on disk
        /// </summary>
        /// <param name="size">The length of the array to allocate</param>
        /// <param name="path">The directory where the memory mapped file is to be stored</param>
        public MappedArray(long size, string path)
        {
            _path = path;
            _pooltimer = new Timer();
            _pooltimer.Elapsed += _pooltimer_Elapsed;
            _pooltimer.Interval = TimeSpan.FromHours(1).TotalMilliseconds;
            _pooltimer.AutoReset = true;
            _pooltimer.Start();

            _fileName = Path.Combine(path, _uniqueName + ".bin");

            // Get the size of TValue
            _dataSize = Marshal.SizeOf(typeof(TValue));

            // Allocate a global buffer for this instance
            _buffer = new byte[_dataSize];
            // Allocate a global unmanaged buffer for this instance
            _memPtr = Marshal.AllocHGlobal(_dataSize);
            SetFileSize(size);
        }
        #endregion

        void _pooltimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (_lockObject)
            {
                foreach (KeyValuePair<int, DateTime> pair in _lastUsedThread)
                {
                    if (pair.Value < DateTime.UtcNow.AddHours(-1))
                    {
                        _inUse[pair.Key].Dispose();
                        _inUse[pair.Key].Close();
                        _inUse.Remove(pair.Key);
                    }
                }
                _lastUsedThread.Clear();
            }
        }



        #region Finalizer
        ~MappedArray()
        {
            Dispose(false);
        }
        #endregion

        #region Private methods
       
        private Stream GetView(int threadId)
        {
            MapViewStream s;
            if (!_inUse.TryGetValue(threadId, out s))
            {
                MapViewStream mvs = _map.MapAsStream();
                lock (_lockObject)
                {
                    _inUse.Add(threadId, mvs);
                }
                return mvs;
            }
            return s;
        }

      
        private void SetFileSize(long size)
        {
            _fileSize = _dataSize * size;
            _map = MemoryMappedFile.Create(_fileName, MapProtection.PageReadWrite, _fileSize);
        }
        #endregion

        #region Public methods
        public void Write(byte[] buffer)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            _lastUsedThread[threadId] = DateTime.UtcNow;

            Stream s = GetView(threadId);
            s.Write(buffer, 0, buffer.Length);
        }

        public void WriteByte(byte b)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            _lastUsedThread[threadId] = DateTime.UtcNow;

            Stream s = GetView(threadId);
            byte[] buffer = new byte[1] { b };
            s.Write(buffer, 0, 1);
        }

        public int Read()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            _lastUsedThread[threadId] = DateTime.UtcNow;

            Stream s = GetView(threadId);
            int count = s.Read(_buffer, 0, _buffer.Length);
            return count;
        }

        public byte ReadByte()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            Stream s = GetView(threadId);

            return (byte)s.ReadByte();
        }

        public TValue this[long index]
        {
            get
            {
                lock (this)
                {
                    if (index >= Length)
                    {
                        throw new ArgumentOutOfRangeException("index", "Tried to access item outside the array boundaries");
                    }
                    Position = index;
                    Read();
                    TValue value = ConvertToTValue();
                    return value;
                }
            }
            set
            {
                lock (this)
                {
                    if (index >= Length)
                    {
                        if (_autogrow)
                            Grow(index, 10);
                        else
                        {
                            throw new ArgumentOutOfRangeException("index", "Tried to access item outside the array");
                        }
                    }
                    Position = index;
                    ConvertToBytes(value);
                    Write(_buffer);
                }
            }
        }

        private void ConvertToBytes(TValue value)
        {
           
            Marshal.StructureToPtr(value, _memPtr, true);
            Marshal.Copy(_memPtr, _buffer, 0, _dataSize);
        }

        private TValue ConvertToTValue()
        {
            Marshal.Copy(_buffer, 0, _memPtr, _dataSize);

            object obj = Marshal.PtrToStructure(_memPtr, typeof(TValue));
            return (TValue)obj;
        }

       
        private void Grow(long size, int percentage)
        {
            _deleteFile = false;

            lock (_lockObject)
            {
                Dispose(true);
                long oldSize = _fileSize;
                _fileSize = (long)((float)size * _dataSize * ((100F + percentage) / 100F));
                if (_fileSize < (oldSize + _dataSize))
                {
                    _fileSize = oldSize + _dataSize;
                }
                _map = MemoryMappedFile.Create(_fileName, MapProtection.PageReadWrite, _fileSize);
            }
        }
        #endregion

        #region Clone Members
      
        public MappedArray<TValue> Clone()
        {
            string copyName = _uniqueName + Guid.NewGuid();
            string currentPath = Path.Combine(_path, copyName + ".bin");

            File.Copy(_fileName, currentPath);
            MappedArray<TValue> current = new MappedArray<TValue>(Length, currentPath);
            return current;
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed && disposing)
            {
                lock (_lockObject)
                {

                    foreach (KeyValuePair<int, MapViewStream> pair in _inUse)
                    {
                        pair.Value.Dispose();
                        pair.Value.Close();
                    }
                    _inUse.Clear();
                    _lastUsedThread.Clear();
                }

                if (_map != null)
                {
                    _map.Close();
                }
            }

            try
            {
                if (_deleteFile)
                {
                    Marshal.DestroyStructure(_memPtr, typeof(TValue)); 
                    Marshal.FreeHGlobal(_memPtr); 

                    if (File.Exists(_fileName)) File.Delete(_fileName);
                }
            }
            catch (MMException)
            {
             
                throw;
            }
            _deleteFile = true;
        }
        #endregion

        #region IEnumerable<TValue> Members

        public IEnumerator<TValue> GetEnumerator()
        {
            lock (this)
            {
                Position = 0;
                for (int i = 0; i < Length; i++)
                {
                    Read();
                    yield return ConvertToTValue();
                }
            }
        }

        #endregion

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
