using System;
using System.IO;
using System.Runtime.InteropServices;

namespace TTG.NavalWar.NWData.Util.MemoryMapping
{
    public class MapViewStream : Stream
    {
        #region Map/View Related Fields

        protected MemoryMappedFile _backingFile;
        protected FileMapAccessType _access = FileMapAccessType.Write;
        protected bool _isWriteable;
        IntPtr _viewBaseAddr = IntPtr.Zero; 
        protected long _mapSize;
        protected long _viewStartIdx = -1;
        protected long _viewSize = -1;
        long _viewPosition = 0;

        #region Properties

        public IntPtr ViewBaseAddr
        {
            get { return _viewBaseAddr; }
        }

        public bool IsViewMapped
        {
            get { return (_viewStartIdx != -1); }
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
            _viewBaseAddr = _backingFile.MapView(_access, ref viewStartIdx, ref viewSize);
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
            _access = isWriteable ? FileMapAccessType.Write : FileMapAccessType.Read;
           
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
            get { return _viewSize; }
        }

        public override long Position
        {
            get { return _viewPosition; }
            set { Seek(value, SeekOrigin.Begin); }
        }

        private bool _isOpen;
        public bool IsOpen { get { return _isOpen; } }

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

            int bytesToRead = (int)Math.Min(Length - _viewPosition, count);

            Marshal.Copy((IntPtr)(_viewBaseAddr.ToInt64() + _viewPosition), buffer, offset, bytesToRead);

            _viewPosition += bytesToRead;
            return bytesToRead;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if ( !IsOpen )
                throw new ObjectDisposedException( "Stream is closed" );
            if ( !CanWrite )
                throw new MMException( "Stream cannot be written to" );

            if ( buffer.Length - offset < count )
                throw new ArgumentException( "Invalid Offset" );

            int bytesToWrite = ( int )Math.Min(Length - _viewPosition, count);
            if ( bytesToWrite == 0 )
                return;

            Marshal.Copy(buffer, offset, (IntPtr)(_viewBaseAddr.ToInt64() + _viewPosition), bytesToWrite);

            _viewPosition += bytesToWrite;
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
          
            if (newpos < 0)
                throw new MMException("Invalid Seek Offset");

            // Map view if position is not in view
            if (newpos < _viewStartIdx || newpos > (_viewStartIdx + _viewSize))
            {
                UnmapView();
                MapView(ref newpos, ref _mapSize);                
            }

            // Move view position
            _viewPosition = newpos - _viewStartIdx;

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

        protected override void Dispose(bool disposing)
        {
            if (_isOpen)
            {
                Flush();
                UnmapView();
                _isOpen = false;
            }

            if (disposing)
                GC.SuppressFinalize(this);

            base.Dispose(disposing);
        }

        ~MapViewStream()
        {
            Dispose(false);
        }

        #endregion
    }
}
