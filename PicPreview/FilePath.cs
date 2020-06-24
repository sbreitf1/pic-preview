namespace PicPreview
{
    public struct FilePath
    {
        public string Path { get; set; }
        public FilePath Dir { get { return (this.Path == null ? null : System.IO.Path.GetDirectoryName(this.Path)); } }
        public string FileName { get { return (this.Path == null ? null : System.IO.Path.GetFileName(this.Path)); } }
        public string FileNameWithoutExtension { get { return (this.Path == null ? null : System.IO.Path.GetFileNameWithoutExtension(this.Path)); } }
        public string Extension { get { return (this.Path == null ? null :  System.IO.Path.GetExtension(this.Path)); } }
        public bool IsEmpty { get { return string.IsNullOrWhiteSpace(this.Path); } }

        public FilePath(string path)
        {
            this.Path = path;
        }

        public override string ToString()
        {
            return this.Path;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is FilePath)
            {
                FilePath other = (FilePath)obj;
                if (other.Path == null && this.Path == null)
                {
                    return true;
                }
                else if (other.Path == null || this.Path == null)
                {
                    return false;
                }
                return other.Path.ToLower().Equals(this.Path.ToLower());
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Path.GetHashCode();
        }

        public static bool operator ==(FilePath lhs, FilePath rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(FilePath lhs, FilePath rhs)
        {
            return !(lhs == rhs);
        }

        public static implicit operator string(FilePath path) => path.Path;
        public static implicit operator FilePath(string path) => new FilePath(path);
    }

}
