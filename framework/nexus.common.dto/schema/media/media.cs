using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using nexus.common.dal;
using nexus.common.cache;
using nexus.common.core;

namespace nexus.common.media
{
    public sealed class media
    {
        private bool   mMediaLocal    = true;
        private bool   mUseGenreDirs  = false;

        private string mBasePath      = "";
        private string mMediaPath     = "";
        private string mContentPath   = "";
        private string mDownloadPath  = "";

        private string mRSSPath       = "";
        private string mImagePath     = "";
        private string mMusicPath     = "";
        private string mVideoPath     = "";
        private string mCoverPath     = "";
        private string mAdvertPath    = "";
        private string mPresentorPath = "";
        private string mVisualsPath   = "";
        private string mSmartPath     = "";
        private string mLibraryPath   = "";
        private string mFunniesPath   = "";

        private static media mInstance = new media();
        public static  media Instance { get { return mInstance; } }

        public void Create(string BasePath, string MediaStr)
        {

            mBasePath = BasePath;

            try
            {
                Array arr = MediaStr.Split(';');
                for (int i = 0; i <= arr.GetLength(0) - 1; i++)
                {
                    if (arr.GetValue(i).ToString().Contains("="))
                    {
                        Array art = arr.GetValue(i).ToString().Split('=');
                        switch (art.GetValue(0).ToString().ToUpper())
                        {
                            case "MEDIAPATH":
                                mMediaPath = art.GetValue(1).ToString();
                                break;
                            case "CONTENTPATH":
                                mContentPath = art.GetValue(1).ToString();
                                break;
                            case "DOWNLOADPATH":
                                mDownloadPath = art.GetValue(1).ToString();
                                break;
                            case "RSSPATH":
                                mRSSPath = art.GetValue(1).ToString();
                                break;
                            case "IMAGEPATH":
                                mImagePath = art.GetValue(1).ToString();
                                break;
                            case "MUSICPATH":
                                mMusicPath = art.GetValue(1).ToString();
                                break;
                            case "VIDEOPATH":
                                mVideoPath = art.GetValue(1).ToString();
                                break;
                            case "COVERPATH":
                                mCoverPath = art.GetValue(1).ToString();
                                break;
                            case "ADVERTPATH":
                                mAdvertPath = art.GetValue(1).ToString();
                                break;
                            case "PRESENTORPATH":
                                mPresentorPath = art.GetValue(1).ToString();
                                break;
                            case "VISUALSPATH":
                                mVisualsPath = art.GetValue(1).ToString();
                                break;
                            case "SMARTPATH":
                                mSmartPath = art.GetValue(1).ToString();
                                break;
                            case "LIBRARYPATH":
                                mLibraryPath = art.GetValue(1).ToString();
                                break;
                            case "FUNNIESPATH":
                                mFunniesPath = art.GetValue(1).ToString();
                                break;
                            case "MEDIALOCAL":
                                mMediaLocal = art.GetValue(1).ToString().ToUpper().Equals("TRUE");
                                break;
                            case "USEGENREDIRS":
                                mUseGenreDirs = art.GetValue(1).ToString().ToUpper().Equals("TRUE");
                                break;
                        }
                    }
                }
                mContentPath = Path.Combine(Path.GetPathRoot(mMediaPath), "Content");
            }
            catch (Exception ex)
            {
            }

        }

        public string GetSettings()
        {

            string SaveStr = "MediaPath=" + mMediaPath;
            SaveStr += ";PlayLocal=" + mMediaLocal.ToString();
            if (mContentPath != "")
                SaveStr += ";ContentPath=" + mContentPath;
            if (mDownloadPath != "")
                SaveStr += ";DownloadPath=" + mDownloadPath;

            if (mImagePath != "")
                SaveStr += ";ImagePath=" + mImagePath;
            if (mMusicPath != "")
                SaveStr += ";MusicPath=" + mMusicPath;
            if (mVideoPath != "")
                SaveStr += ";VideoPath=" + mVideoPath;
            if (mCoverPath != "")
                SaveStr += ";CoverPath=" + mCoverPath;
            if (mAdvertPath != "")
                SaveStr += ";AdvertPath=" + mAdvertPath;
            if (mRSSPath != "")
                SaveStr += ";RSSPath=" + mRSSPath;
            if (mPresentorPath != "")
                SaveStr += ";PresentorPath=" + mPresentorPath;
            if (mVisualsPath != "")
                SaveStr += ";VisualsPath=" + mVisualsPath;
            if (mSmartPath != "")
                SaveStr += ";SmartPath=" + mSmartPath;
            if (mLibraryPath != "")
                SaveStr += ";LibraryPath=" + mLibraryPath;
            if (mFunniesPath != "")
                SaveStr += ";FunniesPath=" + mFunniesPath;
            return SaveStr;

        }

        public bool UseGenreDirs
        {
            get { return mUseGenreDirs; }
        }
        public string GenrePath(string FileType, string Genre)
        {
            return Path.Combine(Path.Combine(mMediaPath, Genre), (FileType == "M") ? "Music": "Video");
        }
        public string TypePath(string FileType)
        {
            return Path.Combine(mMediaPath, (FileType == "M") ? "Music" : "Video");
        }

        public bool MediaLocal
        {
            get { return mMediaLocal; }
            set { mMediaLocal = value; }
        }

        public string MediaPath
        {
            get { return mMediaPath; }
            set { mMediaPath = value; }
        }
        public string ImagePath
        {
            get { return string.IsNullOrEmpty(mImagePath) ? Path.Combine(mMediaPath, "Images") : mImagePath; }
            set { mImagePath = value; }
        }
        public string MusicPath
        {
            get { return string.IsNullOrEmpty(mMusicPath) ? Path.Combine(mMediaPath, "Music") : mMusicPath; }
            set { mMusicPath = value; }
        }
        public string VideoPath
        {
            get { return string.IsNullOrEmpty(mVideoPath) ? Path.Combine(mMediaPath, "Video") : mVideoPath; }
            set { mVideoPath = value; }
        }
        public string CoverPath
        {
            get { return string.IsNullOrEmpty(mCoverPath) ? Path.Combine(mMediaPath, "Covers") : mCoverPath; }
            set { mCoverPath = value; }
        }
        public string PresentorPath
        {
            get { return string.IsNullOrEmpty(mPresentorPath) ? Path.Combine(mBasePath, "Presentors") : mPresentorPath; }
            set { mPresentorPath = value; }
        }

        public string AdvertsPath
        {
            get { return Path.Combine(Path.Combine(string.IsNullOrEmpty(mContentPath) ? Path.GetPathRoot(mMediaPath) : mContentPath, "Adverts"), "Local"); }
            set { mAdvertPath = value; }
        }
        public string RSSPath
        {
            get { return Path.Combine(Path.Combine(Path.Combine(string.IsNullOrEmpty(mContentPath) ? Path.GetPathRoot(mMediaPath) : mContentPath, "Adverts"), "Local"), "RSS"); }
            set { mRSSPath = value; }
        }


        public string VisualsPath
        {
            get { return string.IsNullOrEmpty(mVisualsPath) ? Path.Combine(mContentPath, "Visuals") : mVisualsPath; }
            set { mVisualsPath = value; }
        }
        public string SmartPath
        {
            get { return string.IsNullOrEmpty(mSmartPath) ? Path.Combine(mContentPath, "SmartArt") : mSmartPath; }
            set { mSmartPath = value; }
        }
        public string LibraryPath
        {
            get { return string.IsNullOrEmpty(mLibraryPath) ? Path.Combine(mContentPath, "Library") : mLibraryPath; }
            set { mLibraryPath = value; }
        }
        public string FunniesPath
        {
            get { return string.IsNullOrEmpty(mFunniesPath) ? Path.Combine(mContentPath, "Funnies") : mFunniesPath; }
            set { mFunniesPath = value; }
        }
 
    }
}
