using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MemoryMapping
{
    public class MapViewStream : Stream, IDisposable
    {
        #region Map/View Related Fields

        protected MemoryMappedFile _backingFile;
        protected MapAccess _access = MapAccess.FileMapWrite;
        protected bool _isWriteable;
        IntPtr _viewBaseAddr = IntPtr.Zero; 
        protected long _mapSize;
        protected long _viewStartIdx = -1;
        protected long _viewSize = -1;
        long _position; 


        #region Properties
        public IntPtr ViewBaseAddr
        {
            get { return _viewBaseAddr; }
        }
        public bool IsViewMapped
        {
            get { return (_viewStartIdx != -1) && (_viewStartIdx + _viewSize) <= (_mapSize); }
        }

        #endregion

        #endregion 

        #region Map / Unmap View

        #region Unmap View

        protected void UnmapView()
        {
            if (IsViewMapped)
            {
                _backingFile.UnMapView(this);
                _viewStartIdx = -1;
                _viewSize = -1;
            }
        }

        #endregion

        #region Map View

        protected void MapView(ref long viewStartIdx, ref long viewSize)
        {
           
            _viewBaseAddr = _backingFile.MapView(_access, viewStartIdx, viewSize);
            _viewStartIdx = viewStartIdx;
            _viewSize = viewSize;

        }
        #endregion

        #endregion

        #region Constructors

       
        internal MapViewStream(MemoryMappedFile backingFile, long mapSize, bool isWriteable)
        {
            if (backingFile == null)
            {
                throw new MMException("MapViewStream.MapViewStream - backingFile is null");
            }
            if (!backingFile.IsOpen)
            {
                throw new MMException("MapViewStream.MapViewStream - backingFile is not open");
            }
            if ((mapSize < 1) || (mapSize > backingFile.MaxSize))
            {
                throw new MMException(string.Format("MapViewStream.MapViewStream - mapSize is invalid.  mapSize == {0}, backingFile.MaxSize == {1}", mapSize, backingFile.MaxSize));
            }

            _backingFile = backingFile;
            _isWriteable = isWriteable;
            _access = isWriteable ? MapAccess.FileMapWrite : MapAccess.FileMapRead;
           
            _mapSize = mapSize;

            _isOpen = true;

            

            Seek(0, SeekOrigin.Begin);
        }

        #endregion

        #region Stream Properties

        public override bool CanRead
        {
            get { return true; }
        }
        public override bool CanSeek
        {
            get { return true; }
        }
        public override bool CanWrite
        {
            get { return _isWriteable; }
        }
        public override long Length
        {
            get { return _mapSize; }
        }

        public override long Position
        {
            get { return _position; }
            set { Seek(value, SeekOrigin.Begin); }
        }

        #endregion 

        #region Stream Methods

        public override void Flush()
        {
            if (!IsOpen)
                throw new ObjectDisposedException("Stream is closed");

            
            _backingFile.Flush(this);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!IsOpen)
                throw new ObjectDisposedException("Stream is closed");

            if (buffer.Length - offset < count)
                throw new ArgumentException("Invalid Offset");

            int bytesToRead = (int)Math.Min(Length - _position, count);
            Marshal.Copy((IntPtr)(_viewBaseAddr.ToInt64() + _position), buffer, offset, bytesToRead);

            _position += bytesToRead;
            return bytesToRead;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!IsOpen)
                throw new ObjectDisposedException("Stream is closed");
            if (!CanWrite)
                throw new MMException("Stream cannot be written to");

            if (buffer.Length - offset < count)
                throw new ArgumentException("Invalid Offset");

            int bytesToWrite = (int)Math.Min(Length - _position, count);
            if (bytesToWrite == 0)
                return;

            Marshal.Copy(buffer, offset, (IntPtr)(_viewBaseAddr.ToInt64() + _position), bytesToWrite);

            _position += bytesToWrite;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (!IsOpen)
                throw new ObjectDisposedException("Stream is closed");

            long newpos = 0;
            switch (origin)
            {
                case SeekOrigin.Begin: newpos = offset; break;
                case SeekOrigin.Current: newpos = Position + offset; break;
                case SeekOrigin.End: newpos = Length + offset; break;
            }
          
            if (newpos < 0 || newpos > Length)
                throw new MMException("Invalid Seek Offset");
            _position = newpos;

            if (!IsViewMapped)
            {
                MapView(ref newpos, ref _mapSize); 
            }

            return newpos;
        }

        public override void SetLength(long value)
        {
            
            throw new NotSupportedException("Can't change map size");
        }

        public override void Close()
        {
            Dispose(true);
        }

        #endregion 

        #region IDisposable Implementation

        private bool _isOpen;
        public bool IsOpen { get { return _isOpen; } }

        public new void Dispose()
        {
            Dispose(true);
        }

        protected new virtual void Dispose(bool disposing)
        {
            if (IsOpen)
            {
                Flush();
                UnmapView();
                _isOpen = false;
            }

            if (disposing)
                GC.SuppressFinalize(this);
        }

        ~MapViewStream()
        {
            Dispose(false);
        }

        #endregion

    }
}
