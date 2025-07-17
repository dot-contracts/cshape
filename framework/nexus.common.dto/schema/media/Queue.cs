using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualBasic;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.IO;


using nexus.common;
using nexus.common.dal;
using nexus.common.cache;
using nexus.common.core;

namespace nexus.common.media
{

    public class Queue
    {

        public enum QueueStates  { Announce, Ready, Loaded, Started, Playing, Finishing, Finished, Deleted, Aborted  }


        private string mZoneId;

        private QueueStates mQState = QueueStates.Ready;
        private string mDescription = "";
        private string mQueueType = "";

        private string mArea = "";
        private int mQueueCount = 0;

        private System.DateTime mLastQCheck = DateTime.Now.AddDays(-1);
        private bool mIsXML = false;
        private string mQueueXML = "";
        private int mQueueRow = 0;

        private string mCurrentType = "";
        private int mDuration = 0;
        private int mCueIn = 0;
        private int mCueOut = 0;
        private int mFadeIn = 0;
        private int mFadeOut = 0;
        private int mFadeVolume = 15;
        private int mVolume = 100;
        private int mDefaultVolume = 100;
        private float mRate = 1;

        private int mBalance = 0;
        private int mRowCounter = 0;
        private string mLastQueueId = "";
        private string mQueueId = "";
        private string mTrackId = "";
        private string mMediaFile = "";
        private string mMediaName = "";

        private string mFileType = "";
        private string mTrack = "";
        private string mArtist = "";
        private string mAlbum = "";

        private string mBPM = "";

        private string mStartAt = "";

        private bool mItemsAdded = false;
        private string mPlayListID = "";

        private string mPlayListType = "";
        private float mCPU;
        private float mTotalPhys;
        private float mAvailPhys;
        private float mTotalVirt;

        private float mAvailVirt;

        private Random mRand;
        public event OnChangedEventHandler OnChanged;
        public delegate void OnChangedEventHandler();
        public event OnNextTrackChangedEventHandler OnNextTrackChanged;
        public delegate void OnNextTrackChangedEventHandler(string NextID);
        public event OnStatusEventHandler OnStatus;
        public delegate void OnStatusEventHandler(string Location, string Message);

        public Queue(string QueueType)
        {
            mQueueType = QueueType;
            mDefaultVolume = DefaultVolume;
            mRand = new Random(Convert.ToInt32(System.DateTime.Now.Ticks % System.Int32.MaxValue));
        }

        public Queue(string ZoneId, string QueueType)
        {
            mZoneId = ZoneId;
            mQueueType = QueueType;
            mDefaultVolume = DefaultVolume;
            mRand = new Random(Convert.ToInt32(System.DateTime.Now.Ticks % System.Int32.MaxValue));
        }

        public Queue(string ZoneId, string QueueType, string Area, int DefaultVolume, int CueIn, int CueOut, int FadeIn, int FadeOut)
        {
            mCueIn = CueIn;
            mCueOut = CueOut;
            mFadeIn = FadeIn;
            mFadeOut = FadeOut;

            mArea = Area;
            mZoneId = ZoneId;
            mQueueType = QueueType;
            mDefaultVolume = DefaultVolume;
            mRand = new Random(Convert.ToInt32(System.DateTime.Now.Ticks % System.Int32.MaxValue));
        }

        public void Clear()
        {
            mItemsAdded = false;
        }

        private bool GetFullName(ref string MediaFile, string FileType)
        {
            if (!File.Exists(MediaFile))
            {
                switch (FileType.ToUpper())
                {
                    case "M":
                        MediaFile = Path.Combine(media.Instance.MusicPath, MediaFile);
                        break;
                    case "V":
                        MediaFile = Path.Combine(media.Instance.VideoPath, MediaFile);
                        break;
                    case "R":
                        break;
                    //MediaFile = Path.Combine(oMediaSets.RadioPath, MediaFile)
                    default:
                        break;
                }
            }
            return File.Exists(MediaFile);
        }

        public bool AddPacked(string Packed)
        {

            Array Arr = Packed.Split(';');

            string QueueID   = Arr.GetValue(0).ToString();
            string TrackID   = Arr.GetValue(1).ToString();
            string MediaFile = Arr.GetValue(2).ToString();
            string FileType  = Arr.GetValue(3).ToString();
            if (GetFullName(ref MediaFile, FileType))
            {
                bool Processed = false;
                int PlayOrder = helpers.ToInt(shell.Instance.ExecuteLookUp("dto", "Queue", "Select max(playorder) from media.dbo.Queue where zoneid=" + mZoneId + " and qtype='" + mQueueType + "' and Area='" + mArea + "' and state=" + QueueStates.Ready + " and deleted=0"));

                string SQL = "Insert into media.dbo.queue (state,qtype,zoneid,area,termid,userid,fileid,filename,filetype,track,artist,album,genre,bpm,duration,playtime,playorder) values(" + QueueStates.Ready + ",";
                SQL += "'" + mQueueType + "'," + mZoneId + ",'" + mArea + "'," + shell.Instance.ComputerID + ",0," + TrackID + ",'" + MediaFile + "','" + FileType + "',";
                SQL += "'" + helpers.ValidateNumeric(Arr.GetValue(4).ToString()) + "','" + Arr.GetValue(5).ToString() + "','" + Arr.GetValue(6).ToString() + "','" + Arr.GetValue(7).ToString() + "',";
                SQL += helpers.ValidateNumeric(Arr.GetValue(8).ToString()) + "," +  helpers.ValidateNumeric(Arr.GetValue(9).ToString()) + ",'" +  helpers.msToSecs( helpers.ToInt(Arr.GetValue(9).ToString())) + "'," + PlayOrder + ")";
                if (shell.Instance.ExecuteSQL("dto", "Queue", SQL))
                {
                    Processed = true;
                    mItemsAdded = true;
                }
                return Processed;
            }
            return false;

        }

        public bool AddTrack(string MediaFile, string FileType, string Description)
        {

            bool Processed = false;
            if (GetFullName(ref MediaFile, FileType))
            {
                int PlayOrder =  helpers.ToInt(shell.Instance.ExecuteLookUp("dto", "Queue", "Select max(playorder) from media.dbo.Queue where zoneid=" + mZoneId + " and qtype='" + mQueueType + "' and Area='" + mArea + "' and state=" + QueueStates.Ready + " and deleted=0")) + 1;
                string SQL = "Insert into media.dbo.queue (state,qtype,zoneid,area,termid,filename,filetype,track) values(" + QueueStates.Ready + ",";
                SQL += "'" + mQueueType + "'," + mZoneId + ",'" + mArea + "'," + shell.Instance.ComputerID + ",'" + MediaFile + "','" + FileType + "','" + Description + "')";
                if (shell.Instance.ExecuteSQL("dto", "Queue", SQL))
                {
                    mItemsAdded = true;
                    Processed = true;
                }
            }
            return Processed;

        }


        public bool AddTrack(string TrackID, string MediaFile, string FileType, string Description, string Artist, string Album, string Genre, int BPM, int Duration)
        {
            int PlayOrder =  helpers.ToInt(shell.Instance.ExecuteLookUp("dto", "Queue", "Select max(playorder) from media.dbo.Queue where zoneid=" + mZoneId + " and qtype='BGM' and state=" + QueueStates.Ready + " and deleted=0"));
            return AddTrack(TrackID, MediaFile, FileType, Description, Artist, Album, Genre, BPM, Duration, PlayOrder);
        }

        public bool AddTrack(string TrackID, string MediaFile, string FileType, string Description, string Artist, string Album, string Genre, int BPM, int Duration, int PlayOrder)
        {

            if (GetFullName(ref MediaFile, FileType))
            {
                bool Processed = false;

                string SQL = "Insert into media.dbo.queue (state,qtype,zoneid,area,termid,userid,fileid,filename,filetype,track,artist,album,genre,bpm,duration,playtime,playorder) values(" + QueueStates.Ready + ",";
                SQL += "'" + mQueueType + "'," + mZoneId + ",'" + mArea + "'," + shell.Instance.ComputerID + ",0," + TrackID + ",'" + MediaFile + "','" + FileType + "',";
                SQL += "'" + Description + "','" + Artist + "','" + Album + "','" + Genre + "',";
                SQL += BPM + "," + Duration + ",'" +  helpers.msToSecs(Duration) + "'," + PlayOrder + ")";
                if (shell.Instance.ExecuteSQL("dto", "Queue", SQL))
                {
                    mItemsAdded = true;
                    Processed = true;
                }
                return Processed;
            }
            return false;

        }

        public bool AddBGM(string PlayID, string PlayType, string TrackID, string MediaFile, string FileType, string Description, string Artist, string Album, string Genre, int BPM, int Duration)
        {

            bool Processed = false;

            if (GetFullName(ref MediaFile, FileType))
            {
                int PlayOrder =  helpers.ToInt(shell.Instance.ExecuteLookUp("dto", "Queue", "Select max(playorder) from media.dbo.Queue where zoneid=" + mZoneId + " and qtype='" + mQueueType + "' and Area='" + mArea + "' and state=" + QueueStates.Ready + " and deleted=0")) + 1;

                string QId = shell.Instance.ExecuteLookUp("dto", "Queue", "Select id from media.dbo.Queue where termid=" + shell.Instance.ComputerID + " and zoneid=" + mZoneId + " and qtype='BGM' and Area='' and fileid=" + TrackID);

                if ( helpers.ToInt(QId) == 0)
                {
                    string SQL = "Insert into media.dbo.queue (state,qtype,zoneid,area,termid,userid,fileid,filename,filetype,track,artist,album,genre,bpm,duration,playtime,playlistid,playorder) values(" + QueueStates.Ready + ",";
                    SQL += "'" + mQueueType + "'," + mZoneId + ",'" + mArea + "'," + shell.Instance.ComputerID + ",0," + TrackID + ",'" + MediaFile + "','" + FileType + "',";
                    SQL += "'" + Description + "','" + Artist + "','" + Album + "','" + Genre + "',";
                    SQL += BPM + "," + Duration + ",'" +  helpers.msToSecs(Duration) + "'," + PlayID + ",'" + PlayType + "'," + PlayOrder + ")";
                    if (shell.Instance.ExecuteSQL("dto", "Queue", SQL) )
                    {
                        mItemsAdded = true;
                        Processed = true;
                    }
                }
                else
                {
                    if (shell.Instance.ExecuteSQL("dto", "Queue", "update media.dbo.queue set deleted=0,state=" + QueueStates.Ready + ",playorder=" + PlayOrder + " where id=" + QId) )
                    {
                        mItemsAdded = true;
                        Processed = true;
                    }
                }
            }

            return Processed;

        }

        public bool AddPlayList(DataRow Row, ref int PlayOrder)
        {

            string MediaFile = Row["FileName"].ToString();
            string FileType = Row["FileType"].ToString();
            string TrackID = Row["Id"].ToString();

            try
            {
                if (GetFullName(ref MediaFile, FileType))
                {
                    bool Processed = false;
                    string QId = shell.Instance.ExecuteLookUp("dto", "Queue", "Select id from media.dbo.Queue where termid=" + shell.Instance.ComputerID + " and zoneid=" + mZoneId + " and qtype='BGM' and Area='' and fileid=" + TrackID);
                    if ( helpers.ToInt(QId) == 0)
                    {
                        string Genre = Row["Genre"].ToString() + (!string.IsNullOrEmpty(Row["Category"].ToString()) ? ", " + Row["Category"].ToString() : "");
                        string SQL = "Insert into media.dbo.queue (state,qtype,zoneid,area,termid,fileid,filename,filetype,track,artist,album,genre,bpm,duration,playtime,year,playlistid,playlisttype,playorder) values(" + QueueStates.Ready + ",";
                        SQL += "'" + mQueueType + "'," + mZoneId + ",'" + mArea + "'," + shell.Instance.ComputerID + "," + TrackID + ",'" + MediaFile + "','" + FileType + "',";
                        SQL += "'" + Row["Track"].ToString() + "','" + Row["Artist"].ToString() + "','" + Row["Album"].ToString() + "','" + Genre + "',";
                        SQL +=  helpers.ToInt(Row["BPM"].ToString()) + "," +  helpers.ToInt(Row["Duration"].ToString()) + ",'" +  helpers.msToSecs( helpers.ToInt(Row["Duration"].ToString())) + "'," +  helpers.ToInt(Row["Year"].ToString()) + "," +  helpers.ToInt(Row["PlayID"].ToString()) + ",'" + Row["PlayType"].ToString() + "'," + PlayOrder + ")";
                        if (shell.Instance.ExecuteSQL("dto", "Queue", SQL) )
                        {
                            PlayOrder += 1;
                            mItemsAdded = true;
                            Processed = true;
                        }
                    }
                    else
                    {
                        if (shell.Instance.ExecuteSQL("dto", "Queue", "update media.dbo.queue set deleted=0,state=" + QueueStates.Ready + ",playorder=" + PlayOrder + " where id=" + QId) )
                        {
                            PlayOrder += 1;
                            mItemsAdded = true;
                            Processed = true;
                        }
                    }
                    return Processed;
                }
                else
                {
                    //oMediashell.Instance.ExecuteSQL("Update files set installed=0 where id=" & TrackID)
                }
            }
            catch (Exception ex)
            {
                //InsertLogEntry(mZoneId, "Queue", "AddPlaylist", "1", "ex", MediaFile)
                //InsertLogEntry(mZoneId, "Queue", "AddPlaylist", "2", "ex", ex.ToString)
            }
            return false;

        }

        public bool AddPlayListTable(DataTable Data, bool Append)
        {
            return AddPlayListTable(Data, Append, shell.Instance.ComputerID, mZoneId);
        }

        public bool AddPlayListTable(DataTable Data, bool Append, string TermId, string ZoneId)
        {

            bool Processed = false;
            try
            {
                int PlayOrder =  helpers.ToInt(shell.Instance.ExecuteLookUp("dto", "Queue", "Select max(playorder) from media.dbo.Queue where zoneid=" + ZoneId + " and qtype='BGM' and state=" + QueueStates.Ready + " and deleted=0"));
                if (!Append)
                {
                    shell.Instance.ExecuteSQL("dto", "Queue", "update media.dbo.queue set playorder = playorder + " + PlayOrder + " where zoneid=" + ZoneId + " and qtype='BGM' and state=" + QueueStates.Ready + " and deleted=0");
                    PlayOrder = 0;
                }
                for (int i = 0; i <= Data.Rows.Count - 1; i++)
                {
                    string MediaFile = "";
                    string MediaID = "";
                    string FileType = "";
                    try
                    {
                        MediaFile = Data.Rows[i]["FileName"].ToString();
                        FileType = Data.Rows[i]["FileType"].ToString();
                        MediaID = Data.Rows[i]["TrackId"].ToString();

                        if (GetFullName(ref MediaFile, FileType))
                        {
                            string QId = shell.Instance.ExecuteLookUp("dto", "Queue", "Select id from media.dbo.Queue where termid=" + TermId + " and zoneid=" + ZoneId + " and qtype='BGM' and Area='' and fileid=" + MediaID + " and playlistid=" +  helpers.ToInt(Data.Rows[i]["PlayID"].ToString()) + " and playlisttype='" + Data.Rows[i]["PlayType"].ToString() + "'");
                            if ( helpers.ToInt(QId) == 0)
                            {
                                string Genre = Data.Rows[i]["Genre"].ToString() + (!string.IsNullOrEmpty(Data.Rows[i]["Category"].ToString()) ? ", " + Data.Rows[i]["Category"].ToString() : "");
                                string SQL = "Insert into media.dbo.queue (state,qtype,zoneid,termid,fileid,filename,filetype,track,artist,album,genre,bpm,duration,playtime,year,playlistid,playlisttype,playorder,area) values(" + QueueStates.Ready + ",";
                                SQL += "'BGM'," + ZoneId + "," + TermId + "," + MediaID + ",'" + MediaFile + "','" + Data.Rows[i]["FileType"].ToString() + "',";
                                SQL += "'" + Data.Rows[i]["Track"].ToString() + "','" + Data.Rows[i]["Artist"].ToString() + "','" + Data.Rows[i]["Album"].ToString() + "','" + Genre + "',";
                                SQL +=  helpers.ToInt(Data.Rows[i]["BPM"].ToString()) + "," +  helpers.ToInt(Data.Rows[i]["Duration"].ToString()) + ",'" +  helpers.msToSecs( helpers.ToInt(Data.Rows[i]["Duration"].ToString())) + "'," +  helpers.ToInt(Data.Rows[i]["Year"].ToString()) + "," +  helpers.ToInt(Data.Rows[i]["PlayID"].ToString()) + ",'" + Data.Rows[i]["PlayType"].ToString() + "'," + PlayOrder + ",'')";
                                if (shell.Instance.ExecuteSQL("dto", "Queue", SQL) )
                                {
                                    PlayOrder += 1;
                                    mItemsAdded = true;
                                    Processed = true;
                                }
                                else
                                {
                                    //shell.Instance.InsertLogEntry(true, "0", "", "Queue", "AddPlay", "None In Playlist", SQL);
                                }
                            }
                            else
                            {
                                if (shell.Instance.ExecuteSQL("dto", "Queue", "update media.dbo.queue set deleted=0,state=" + QueueStates.Ready + ",playorder=" + PlayOrder.ToString() + " where id=" + QId))
                                {
                                    PlayOrder += 1;
                                    mItemsAdded = true;
                                    Processed = true;
                                }
                            }
                        }
                        else if (shell.Instance.ComputerType == "MSVR")
                        {
                            shell.Instance.ExecuteSQL("dto", "Queue", "update media.dbo.files set installed=0 where id=" + MediaID);
                        }
                    }
                    catch (Exception ex)
                    {
                        //InsertLogEntry(mZoneId, "Queue", "AddPlaylistTB", "1", "ex", MediaFile)
                        //InsertLogEntry(mZoneId, "Queue", "AddPlaylistTB", "2", "ex", ex.ToString)
                    }
                }
            }
            catch (Exception ex)
            {
                //InsertLogEntry(mZoneId, "Queue", "AddPlaylistTB", "3", "ex", ex.ToString)
            }
            return Processed;

        }

        public bool AddJukebox(DataRow Row)
        {

            string MediaFile = Row["FileName"].ToString();
            string FileType = Row["FileType"].ToString();

            if (GetFullName(ref MediaFile, FileType))
            {
                bool Processed = false;
                int PlayOrder =  helpers.ToInt(shell.Instance.ExecuteLookUp("dto", "Queue", "Select max(playorder) from media.dbo.Queue where zoneid=" + mZoneId + " and qtype='" + mQueueType + "' and Area='" + mArea + "' and state=" + QueueStates.Ready + " and deleted=0")) + 1;
                string SQL = "Insert into media.dbo.queue (state,qtype,zoneid,area,termid,userid,fileid,filename,filetype,track,artist,album,genre,bpm,duration,playtime,playorder) values(" + QueueStates.Ready + ",";
                SQL += "'" + mQueueType + "'," + mZoneId + ",'" + mArea + "'," + shell.Instance.ComputerID + ",0," + TrackID + ",'" + MediaFile + "','" + FileType + "',";
                SQL += "'" + Row["Track"].ToString() + "','" + Row["Artist"].ToString() + "','" + Row["Album"].ToString() + "','" + Row["Genre"].ToString() + "',";
                SQL +=  helpers.ToInt(Row["BPM"].ToString()) + "," +  helpers.ToInt(Row["Duration"].ToString()) + ",'" +  helpers.msToSecs( helpers.ToInt(Row["Duration"].ToString())) + "'," + PlayOrder + ")";
                if (shell.Instance.ExecuteSQL("dto", "Queue", SQL))
                {
                    mItemsAdded = true;
                    Processed = true;
                }
                return Processed;
            }
            return false;

        }

        public bool AddAdvert(string CampainID, string FileID, string MediaFile, string FileType, string Description, int Duration, bool ResetState)
        {

            bool Processed = false;

            if (GetFullName(ref MediaFile, FileType))
            {
                int PlayOrder =  helpers.ToInt(shell.Instance.ExecuteLookUp("dto", "Queue", "Select max(playorder) from media.dbo.Queue where zoneid=" + mZoneId + " and qtype='" + mQueueType + "' and Area='" + mArea + "' and state=" + QueueStates.Ready + " and deleted=0")) + 1;
                string QId = shell.Instance.ExecuteLookUp("dto", "Queue", "Select id from media.dbo.Queue where playlistid=" +  helpers.ToInt(CampainID) + " and zoneid=" + mZoneId + " and qtype='" + mQueueType + "' and Area='" + mArea + "' and filetype='" + FileType + "' and filename='" + MediaFile + "'");
                if ( helpers.ToInt(QId) == 0)
                {
                    string SQL = "Insert into media.dbo.queue (state,qtype,zoneid,area,termid,userid,fileid,filename,filetype,track,duration,playtime,playlistid,playorder) values(" + QueueStates.Ready + ",";
                    SQL += "'" + mQueueType + "'," + mZoneId + ",'" + mArea + "'," + shell.Instance.ComputerID + ",0," +  helpers.ToInt(FileID) + ",'" + MediaFile + "','" + FileType + "',";
                    SQL += "'" + Description + "'," + Duration + ",'" +  helpers.msToSecs(Duration) + "'," +  helpers.ToInt(CampainID) + "," + PlayOrder + ")";
                    if (shell.Instance.ExecuteSQL("dto", "Queue", SQL) )
                    {
                        mItemsAdded = true;
                        Processed = true;
                    }
                }
                else
                {
                    if (shell.Instance.ExecuteSQL("dto", "Queue", "update media.dbo.queue set fileid=" + FileID + ",deleted=0" + (ResetState ? ",state=" + QueueStates.Ready : "") + ",playorder=" + PlayOrder + " where id=" + QId) )
                    {
                        mItemsAdded = true;
                        Processed = true;
                    }
                }
            }

            return Processed;


        }

        public string AddAnnounce(string AnnounceID, string ScheduleID, string MediaFile, string FileType, string Description, string StartsAt, int Duration)
        {

            //Dim QId As String = shell.Instance.ExecuteLookUp("Queue", "Select id from media.dbo.Queue where zoneid=" & mZoneId & " and qtype='" & mQueueType & "' and Area='" & mArea & "' and filetype='" & FileType & "' and filename='" & MediaFile & "' and StartAt='" & StartsAt & "'")
            //If Now.Subtract(StartsAt).TotalMinutes <= 1 Then
            //    If Val(QId) = 0 Then
            //        Dim SQL As String = "Insert into media.dbo.queue (state,qtype,zoneid,area,termid,userid,fileid,filename,filetype,track,duration,playtime,StartAt,playlistid) values(" & QueueStates.Announce & ","
            //        SQL &= "'Announce'," & mZoneId & ",'" & mArea & "'," & shell.Instance.ComputerID & ",0," & Val(AnnounceID) & ",'" & MediaFile & "','" & FileType & "',"
            //        SQL &= "'" & Description & "'," & Val(Duration) & ",'" & Props.Instance.msToSecs(Val(Duration)) & "','" & StartsAt & "'," & Val(ScheduleID) & ")"
            //        If shell.Instance.ExecuteSQL("Queue", SQL) = 1 Then
            //            mItemsAdded = True
            //            Processed = True
            //        End If
            //    Else
            //        If shell.Instance.ExecuteSQL("Queue", "update media.dbo.queue set state=" & QueueStates.Announce & ",deleted=0 where id=" & QId) = 1 Then
            //            mItemsAdded = True
            //            Processed = True
            //        End If
            //    End If
            //Else
            //    If Val(QId) > 0 Then shell.Instance.ExecuteSQL("Queue", "update media.dbo.queue set state=" & QueueStates.Deleted & " where id=" & QId)
            //End If

            string QId = shell.Instance.ExecuteLookUp("dto", "Queue", "Select id from media.dbo.Queue where zoneid=" + mZoneId + " and qtype='Announce' and Area='" + mArea + "' and filetype='" + FileType + "' and filename='" + MediaFile + "' and scheduleid=" + ScheduleID);
            //If Now.Subtract(StartsAt).TotalMinutes <= 1 Then
            if ( helpers.ToInt(QId) == 0)
            {
                string SQL = "Insert into media.dbo.queue (state,qtype,zoneid,area,termid,userid,fileid,filename,filetype,track,duration,playtime,StartAt,scheduleid) values(" + QueueStates.Announce + ",";
                SQL += "'Announce'," + mZoneId + ",'" + mArea + "'," + shell.Instance.ComputerID + ",0," +  helpers.ToInt(AnnounceID) + ",'" + MediaFile + "','" + FileType + "',";
                SQL += "'" + Description + "'," + Duration + ",'" +  helpers.msToSecs(Duration) + "','" + StartsAt + "'," +  helpers.ToInt(ScheduleID) + ")";
                if (shell.Instance.ExecuteSQL("dto", "Queue", SQL) )
                {
                    mItemsAdded = true;
                    QId = shell.Instance.ExecuteLookUp("dto", "Queue", "Select id from media.dbo.Queue where zoneid=" + mZoneId + " and qtype='Announce' and Area='" + mArea + "' and filetype='" + FileType + "' and filename='" + MediaFile + "'");
                }
            }
            else
            {
                if (shell.Instance.ExecuteSQL("dto", "Queue", "update media.dbo.queue set StartAt='" + StartsAt + "',state=" + QueueStates.Announce + ",deleted=0 where id=" + QId) )
                {
                    mItemsAdded = true;
                }
            }

            return QId;


        }

        public bool AddSmartArt(string Campain, string MediaFile, string Description)
        {
            bool Processed = false;

            if (File.Exists(MediaFile))
            {
                int PlayOrder =  helpers.ToInt(shell.Instance.ExecuteLookUp("dto", "Queue", "Select max(playorder) from media.dbo.Queue where zoneid=" + mZoneId + " and qtype='" + mQueueType + "' and Area='" + mArea + "' and state=" + QueueStates.Ready + " and deleted=0")) + 1;
                string SQL = "Insert into media.dbo.queue (state,qtype,zoneid,area,termid,userid,fileid,filename,filetype,track,duration,playtime,playlistid,playorder) values(" + QueueStates.Ready + ",";
                SQL += "'" + mQueueType + "'," + mZoneId + ",'" + mArea + "'," + shell.Instance.ComputerID + ",0," + TrackID + ",'" + MediaFile + "','SmartArt',";
                SQL += "'" + Description + "',0,''," +  helpers.ToInt(Campain) + "," + PlayOrder + ")";
                if (shell.Instance.ExecuteSQL("dto", "Queue", SQL) )
                {
                    mItemsAdded = true;
                    Processed = true;
                }
            }
            return Processed;

        }

        public bool GetNextFile()
        {

            bool Processed = false;
            try
            {
                mLastQueueId = mQueueId;
                string SQL = "Select top 1 Id from media.dbo.Queue where deleted=0 and zoneid=" + mZoneId + " and Area='" + mArea + "' and ";
                switch (mQueueType.ToUpper())
                {
                    case "JUKE":
                        SQL += "(qtype='Juke' or qtype='Announce') and state=" + QueueStates.Ready + " order by id asc";
                        break;
                    case "ANNOUNCE":
                        SQL += "qtype='Announce' and state=" + QueueStates.Announce + " order by startat asc";
                        break;
                    default:
                        SQL += "qtype='" + mQueueType + "' and state=" + QueueStates.Ready + " order by playorder asc";
                        break;
                }

                string QueueId = shell.Instance.ExecuteLookUp("dto", "Queue", SQL);
                if ( helpers.ToInt(QueueId) > 0)
                {
                    mQueueId = QueueId;

                    DataTable DT = shell.Instance.GetDataTable("dto", "Queue", "Select Id,qtype,zoneid,area,termid,userid,fileid,filename,filetype,track,artist,album,genre,bpm,duration,playtime,playlistid,playlisttype,startat from media.dbo.queue where id=" + mQueueId);
                    if (DT.Rows.Count > 0)
                    {
                        mCurrentType = DT.Rows[0]["qType"].ToString();
                        mTrackId = DT.Rows[0]["FileID"].ToString();
                        mFileType = DT.Rows[0]["FileType"].ToString();
                        mPlayListType = DT.Rows[0]["PlayListType"].ToString();

                        mMediaFile = DT.Rows[0]["FileName"].ToString();
                        if (!File.Exists(mMediaFile))
                        {
                            mMediaFile = Path.GetFileName(mMediaFile);
                            GetFullName(ref mMediaFile, mFileType);
                        }

                        mMediaName = Path.GetFileName(mMediaFile);

                        if (File.Exists(mMediaFile))
                        {
                            mTrack = DT.Rows[0]["Track"].ToString();
                            mDuration =  helpers.ToInt(DT.Rows[0]["Duration"].ToString());
                            mBPM = DT.Rows[0]["BPM"].ToString();
                            mArtist = DT.Rows[0]["Artist"].ToString();
                            mAlbum = DT.Rows[0]["Album"].ToString();

                            //mCueIn = Val(DT.Rows(0).Item("CueIn").ToString)
                            //mFadeIn = Val(DT.Rows(0).Item("FadeIn").ToString)
                            //mCueOut = Val(DT.Rows(0).Item("CueOut").ToString)
                            //mFadeOut = Val(DT.Rows(0).Item("FadeOut").ToString)

                            mStartAt = DT.Rows[0]["StartAt"].ToString();
                            mPlayListID = DT.Rows[0]["PlayListID"].ToString();

                            //Rate = Val(mQueue.Rows(mQueueRow).Item("Rate").ToString)
                            //If Rate < 0 Then
                            //    Rate = (1 - (-1 * Rate) / 100)
                            //Else
                            //    Rate = 1 + (1 * Rate / 100)
                            //End If
                            //mVolume = Val(mQueue.Rows(mQueueRow).Item("Volume").ToString)
                            if (mVolume <= 0)
                            {
                                mVolume = mDefaultVolume;
                            }

                            Processed = true;
                        }
                        else
                        {
                            SQL = "Update media.dbo.queue set state=" + QueueStates.Deleted + ", deleted=1 where id=" + mQueueId + " ";
                            SQL += "Select fileid,playlistid,scheduleid from media.dbo.queue where id=" + mQueueId;
                            DT = shell.Instance.GetDataTable("dto", "QFinish", SQL);
                            if (DT.Rows.Count > 0)
                            {
                                string PlayID = DT.Rows[0]["PlayListID"].ToString();
                                if ( helpers.ToInt(PlayID) > 0)
                                {
                                    switch (mPlayListType.ToUpper())
                                    {
                                        case "LOCAL":
                                            shell.Instance.ExecuteSQL("dto", "Queue", "Update media.dbo.locallist set played=1 where Id=" + PlayID);
                                            break;
                                        case "SYSTEM":
                                            shell.Instance.ExecuteSQL("dto", "Queue", "Update media.dbo.playlist set played=1 where Id=" + PlayID);
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    DT.Dispose();
                    DT = null;
                }

                return Processed;

            }
            catch (Exception ex)
            {
                if (OnStatus != null)
                {
                    OnStatus("Q GetNextTrack", ex.Message);
                }
            }
            return false;

        }

        public bool TrackExists(string TrackID)
        {

            bool Processed = false;
            switch (mQueueType.ToUpper())
            {
                case "BGM":
                    Processed =  helpers.ToInt(shell.Instance.ExecuteLookUp("dto", "Queue", "Select Id from media.dbo.Queue where fileid=" + TrackID + " and zoneid=" + mZoneId + " and qtype='BGM' and state=" + QueueStates.Ready + " and deleted=0")) > 0;
                    break;
                case "JUKE":
                    Processed =  helpers.ToInt(shell.Instance.ExecuteLookUp("dto", "Queue", "Select Id from media.dbo.Queue where fileid=" + TrackID + " and zoneid=" + mZoneId + " and qtype='Juke' and state<" + QueueStates.Finished + " and deleted=0")) > 0;
                    break;
                default:
                    Processed = false;
                    break;
            }
            return Processed;

        }

        public void Loaded(string QueueID)
        {
            string SQL = "Update media.dbo.Queue Set state=" + QueueStates.Finished + " where qtype='" + mQueueType + "' and zoneid=" + mZoneId + " and state=" + QueueStates.Loaded + " ";
            SQL += "Update media.dbo.Queue Set state=" + QueueStates.Loaded + " Where Id=" +  helpers.ToInt(QueueID);
            shell.Instance.ExecuteSQL("dto", "QLoad", SQL);
        }
        public void Started(string QueueID)
        {
            shell.Instance.ExecuteSQL("dto", "QStart", "Update media.dbo.Queue Set state=" + QueueStates.Started + ",CPU=" + mCPU.ToString() + ",AvailPhys=" + mAvailPhys.ToString() + ",AvailVirt=" + mAvailVirt.ToString() + ",PlayedOn=getdate() Where Id=" +  helpers.ToInt(QueueID));
        }

        public void Aborted(string QueueID)
        {
            string FileID = "";
            string Fails = "";
            string PlayID = "";

            string SQL = "Update media.dbo.Queue Set state=" + QueueStates.Aborted + ", finishedat=getdate() Where Id=" +  helpers.ToInt(QueueID) + " ";
            SQL += "Select q.fileid,f.loadfails,q.playlistid from media.dbo.Queue as q left join media.dbo.files as f on q.fileid=f.id where q.id=" + QueueID;
            DataTable DT = shell.Instance.GetDataTable("dto", "QFinish", SQL);
            if (DT.Rows.Count > 0)
            {
                FileID = DT.Rows[0]["FileID"].ToString();
                Fails = DT.Rows[0]["LoadFails"].ToString();
                PlayID = DT.Rows[0]["PlayListID"].ToString();
            }
            DT.Dispose();
            DT = null;

            try
            {
                int LoadFail =  helpers.ToInt(Fails);
                if (LoadFail > 7)
                {
                    //shell.Instance.ExecuteSQL("Queue", "Update media.dbo.files Set installed=0 Where Id=" & FileID)
                    //If File.Exists(mMediaFile) Then
                    //    Dim AbPath As String = Path.Combine(EndPoint.Instance.Media.MediaPath, "Aborted")
                    //    If Not Directory.Exists(AbPath) Then Directory.CreateDirectory(AbPath)
                    //    File.Move(mMediaFile, Path.Combine(AbPath, mMediaName))
                    //    Props.Instance.InsertLogEntry(True, mZoneId, "Queue", "Aborted", "File Moved", "", mMediaFile)
                    //End If
                }
                else
                {
                    shell.Instance.ExecuteSQL("dto", "Queue", "Update media.dbo.files Set lastfail=getdate(),loadfails=" + (LoadFail + 1).ToString() + "' Where Id=" + FileID);
                }

                //Props.Instance.InsertLogEntry(true, mZoneId, "Queue", "Aborted", "File Aborted", "", mMediaFile);

                switch (mPlayListType.ToUpper())
                {
                    case "LOCAL":
                        if ( helpers.ToInt(PlayID) > 0)
                        {
                            shell.Instance.ExecuteSQL("dto", "Queue", "Update media.dbo.locallist set played=1 where Id=" + PlayID);
                        }
                        break;
                    case "SYSTEM":
                        if ( helpers.ToInt(PlayID) > 0)
                        {
                            shell.Instance.ExecuteSQL("dto", "Queue", "Update media.dbo.playlist set played=1 where Id=" + PlayID);
                        }
                        break;
                }


            }
            catch (Exception ex)
            {
                //Props.Instance.InsertLogEntry(true, mZoneId, "Queue", "Aborted", "ex", ex.Message);
            }

        }
        public void Finishing(string QueueID)
        {
            shell.Instance.ExecuteSQL("dto", "QFinish", "Update media.dbo.Queue Set state=" + QueueStates.Finishing + " Where Id=" +  helpers.ToInt(QueueID));
        }
        public void SetPosition(string QueueID, int Position)
        {
            shell.Instance.ExecuteSQL("dto", "QPos", "Update media.dbo.Queue Set state=" + QueueStates.Playing + ",position=" + Position + " Where Id=" +  helpers.ToInt(QueueID));
        }

        public void Finished(string QueueID)
        {

            if ( helpers.ToInt(QueueID) > 0)
            {
                string FileID = "";
                string PlayID = "";
                string ScedID = "";

                int Plays = 0;
                string PlayArea = "1";
                string PlayField = "";

                string SQL = "Update media.dbo.Queue Set state=" + QueueStates.Finished + ",CPU=" + mCPU.ToString() + ",AvailPhys=" + mAvailPhys.ToString() + ",AvailVirt=" + mAvailVirt.ToString() + ",FinishedAt=getdate() Where Id=" +  helpers.ToInt(QueueID) + " ";
                SQL += "Update media.dbo.Queue Set state=" + QueueStates.Finished + " where qtype='" + mQueueType + "' and zoneid=" + mZoneId + " and state=" + QueueStates.Playing + " ";
                SQL += "Select fileid,playlistid,scheduleid from media.dbo.queue where id=" + QueueID;
                DataTable DT = shell.Instance.GetDataTable("dto", "QFinish", SQL);
                if (DT.Rows.Count > 0)
                {
                    FileID = DT.Rows[0]["FileID"].ToString();
                    PlayID = DT.Rows[0]["PlayListID"].ToString();
                    ScedID = DT.Rows[0]["scheduleid"].ToString();
                }
                DT.Dispose();
                DT = null;

                switch (mQueueType.ToUpper())
                {
                    case "JUKE":
                        if (mCurrentType.ToUpper() == "ANNOUNCE")
                        {
                            shell.Instance.ExecuteSQL("dto", "Queue", "Update media.dbo.Schedule Set lastplay=getdate() Where Id=" +  helpers.ToInt(ScedID));
                        }
                        else
                        {
                            PlayArea = "2";
                            PlayField = "fileid";
                            Plays =  helpers.ToInt(shell.Instance.ExecuteLookUp("dto", "Queue", "select plays from media.dbo.files where Id=" + FileID)) + 1;
                            Plays = (Plays > 32000 ? 1 : Plays);
                            shell.Instance.ExecuteSQL("dto", "Queue", "Update media.dbo.files Set plays=" + Plays + ",LastPlayed=getdate() Where Id=" + FileID);
                        }
                        break;
                    case "BGM":
                        if ( helpers.ToInt(FileID) > 0)
                        {
                            Plays =  helpers.ToInt(shell.Instance.ExecuteLookUp("dto", "Queue", "select plays from media.dbo.files where Id=" + FileID)) + 1;
                            Plays = (Plays > 32000 ? 1 : Plays);
                            shell.Instance.ExecuteSQL("dto", "Queue", "Update media.dbo.files Set plays=" + Plays + ",LastPlayed=getdate() Where Id=" + FileID);
                            shell.Instance.ExecuteSQL("dto", "BGMLog", "Insert into media.dbo.medialog (logdate,logtype,zoneid,mediaid) values(getdate(),'BGM'," + mZoneId + "," + FileID + ")");
                        }

                        PlayField = "fileid";
                        switch (mPlayListType.ToUpper())
                        {
                            case "LOCAL":
                                if ( helpers.ToInt(PlayID) > 0)
                                {
                                    Plays =  helpers.ToInt(shell.Instance.ExecuteLookUp("dto", "Queue", "select plays from media.dbo.locallist where Id=" + PlayID)) + 1;
                                    Plays = (Plays > 32000 ? 1 : Plays);
                                    shell.Instance.ExecuteSQL("dto", "Queue", "Update media.dbo.locallist set LastPlayed=getdate(),plays=" + Plays + ",played=1 where Id=" + PlayID);
                                }
                                break;
                            case "SYSTEM":
                                if ( helpers.ToInt(PlayID) > 0)
                                {
                                    Plays =  helpers.ToInt(shell.Instance.ExecuteLookUp("dto", "Queue", "select plays from media.dbo.playlist where Id=" + PlayID)) + 1;
                                    Plays = (Plays > 32000 ? 1 : Plays);
                                    if ( helpers.ToInt(shell.Instance.ExecuteLookUp("dto", "Queue", "Update media.dbo.playlist set LastPlayed=getdate(),plays=" + Plays + ",played=1 where Id=" + PlayID + " Select Id from media.dbo.playlist where Id=" + PlayID)) == 0)
                                    {
                                        Debug.WriteLine("");
                                    }
                                }
                                break;
                        }

                        break;
                    case "CAMPAIN":
                    case "ADVERTS":
                    case "VISUALS":
                        if ( helpers.ToInt(FileID) > 0)
                        {
                            try
                            {
                                Plays =  helpers.ToInt(shell.Instance.ExecuteLookUp("dto", "Queue", "select plays from media.dbo.advert where Id=" + FileID)) + 1;
                            }
                            catch (Exception ex)
                            {
                                Plays = 1;
                            }
                            Plays = (Plays > 32000 ? 1 : Plays);
                            shell.Instance.ExecuteSQL("dto", "Queue", "Update media.dbo.advert Set plays=" + Plays + ",LastPlayed=getdate() Where Id=" + FileID);
                        }

                        break;

                        //If Val(mPlayListID) > 0 Then
                        //    Dim Finished As Boolean = True
                        //    For j As Integer = 0 To mQueue.Rows.Count - 1
                        //        If mQueue.Rows(j).Item("PlayId").ToString = Campain And Val(mQueue.Rows(j).Item("PlayState").ToString) < 2 Then
                        //            Finished = False
                        //            Exit For
                        //        End If
                        //    Next
                        //    If Finished Then
                        //        Plays = Val(shell.Instance.ExecuteLookUp("select plays from campain where Id=" & mQueue.Rows(i).Item("playid").ToString)) + 1
                        //        Dim Rows As Integer = shell.Instance.ExecuteSQL("Update media.dbo.campain set LastPlayed=getdate(),plays=" & Plays & ",played=1 where Id=" & mQueue.Rows(i).Item("playid"))
                        //        PlayField = "campainid"
                        //    End If
                        //End If
                        //SaveXML()

                }


                if (!string.IsNullOrEmpty(PlayField) & !string.IsNullOrEmpty(FileID))
                {
                    string PlaysID = shell.Instance.ExecuteLookUp("dto", "Plays1", "select id from media.dbo.plays where areaid=" + PlayArea + " and zoneid=" + mZoneId + " and terminalid=" + shell.Instance.ComputerID + " and " + PlayField + "=" + FileID);
                    if ( helpers.ToInt(PlaysID) == 0)
                    {
                        shell.Instance.ExecuteSQL("dto", "Plays2", "Insert into media.dbo.plays (siteid,terminalid,groupid,zoneid,areaid,plays,lastplayed," + PlayField + ") values(" +  helpers.ToInt(shell.Instance.VenueID) + "," +  helpers.ToInt(shell.Instance.ComputerID) + "," +  helpers.ToInt(shell.Instance.GroupId) + "," + mZoneId + "," + PlayArea + ",1,getdate()," + FileID + ")");
                    }
                    else
                    {
                        Plays =  helpers.ToInt(shell.Instance.ExecuteLookUp("dto", "Plays3", "select plays from media.dbo.plays where id=" +  helpers.ToInt(PlaysID))) + 1;
                        Plays = (Plays > 32000 ? 1 : Plays);
                        shell.Instance.ExecuteSQL("dto", "Plays4", "update media.dbo.plays set plays=" + Plays + ",lastplayed=getdate() where id=" + PlaysID);
                    }
                }

            }
        }

        public void Reset()
        {
            switch (mQueueType.ToUpper())
            {
                case "BGM":
                    break;
                // shell.Instance.ExecuteSQL("Queue", "Update media.dbo.Queue Set state=" & QueueStates.Deleted & ", FinishedAt=getdate() Where qtype='" & mQueueType & "' and Area='" & mArea & "' and ZoneID=" & mZoneId & " And state<" & QueueStates.Finished)
                case "QUEUE":
                    shell.Instance.ExecuteSQL("dto", "Queue", "Update media.dbo.Queue Set state=" + QueueStates.Deleted + ", FinishedAt=getdate() Where qtype='" + mQueueType + "' and Area='" + mArea + "' and ZoneID=" + mZoneId + " And state<" + QueueStates.Finished);
                    break;
                case "ADVERTS":
                case "VISUALS":
                    shell.Instance.ExecuteSQL("dto", "Queue", "Update media.dbo.Queue Set deleted=1, state=" + QueueStates.Deleted + ", FinishedAt=getdate() Where qtype='" + mQueueType + "' and Area='" + mArea + "' and ZoneID=" + mZoneId);
                    break;
                default:
                    if (File.Exists(mQueueXML))
                        File.Delete(mQueueXML);
                    break;
            }
            Clear();
        }

        public string QueueType
        {
            get { return mQueueType; }
            set { mQueueType = value; }
        }
        public int QueueRow
        {
            get { return mQueueRow; }
            set { mQueueRow = value; }
        }

        public int QueueCount
        {
            get
            {
                switch (mQueueType.ToUpper())
                {
                    case "BGM":
                        if (DateTime.Now.Subtract(mLastQCheck).TotalSeconds > 10)
                        {
                            mLastQCheck = DateTime.Now;
                            mQueueCount =  helpers.ToInt(shell.Instance.ExecuteLookUp("dto", "BGMQC", "Select count(Id) from media.dbo.Queue where zoneid=" + mZoneId + " and qtype='BGM' and state=" + QueueStates.Ready + " and deleted=0"));
                        }
                        break;
                    case "JUKE":
                        if (DateTime.Now.Subtract(mLastQCheck).TotalSeconds > 5)
                        {
                            mLastQCheck = DateTime.Now;
                            mQueueCount =  helpers.ToInt(shell.Instance.ExecuteLookUp("dto", "JUKEQC", "Select count(Id) from media.dbo.Queue where zoneid=" + mZoneId + " and state=" + QueueStates.Ready + " and (qtype='Juke' or qtype='Announce') and deleted=0"));
                        }
                        break;
                    case "ADVERTS":
                    case "VISUALS":
                        if (DateTime.Now.Subtract(mLastQCheck).TotalSeconds > 10)
                        {
                            mLastQCheck = DateTime.Now;
                            mQueueCount =  helpers.ToInt(shell.Instance.ExecuteLookUp("dto", "VISQC", "Select count(Id) from media.dbo.Queue where zoneid=" + mZoneId + " and qtype='" + mQueueType + "' and Area='" + mArea + "' and state=" + QueueStates.Ready + " and deleted=0"));
                            //If mQueueCount = 0 Then
                            //    If Val(shell.Instance.ExecuteLookUp("Select count(Id) from media.dbo.Queue where zoneid=" & mZoneId & " and qtype='Adverts' and Area='" & mArea & "' and state=4 and deleted=0")) > 0 Then
                            //        shell.Instance.ExecuteSQL("Update media.dbo.queue set state=0 where zoneid=" & mZoneId & " and qtype='Adverts' and Area='" & mArea & "' and state=4 and deleted=0")
                            //        mQueueCount = Val(shell.Instance.ExecuteLookUp("Select count(Id) from media.dbo.Queue where zoneid=" & mZoneId & " and qtype='Adverts' and Area='" & mArea & "' and state=0 and deleted=0"))
                            //    End If
                            //End If
                        }
                        break;
                    case "ANNOUNCE":
                        if (DateTime.Now.Subtract(mLastQCheck).TotalSeconds > 10)
                        {
                            mLastQCheck = DateTime.Now;
                            mQueueCount =  helpers.ToInt(shell.Instance.ExecuteLookUp("dto", "ANNQC", "Select count(Id) from media.dbo.Queue where zoneid=" + mZoneId + " and qtype='Announce' and state=" + QueueStates.Announce + " and deleted=0"));
                        }
                        break;
                }
                return mQueueCount;
            }
            set
            {
                mLastQCheck = DateTime.Now.AddDays(-1);
                mQueueCount = value;
            }
        }

        public bool MoveNext(string QueueID)
        {
            bool Processed = false;
            DataTable DT = shell.Instance.GetDataTable("dto", "Queue", "Select Id from media.dbo.queue where state=" + QueueStates.Ready + " and deleted=0 and id <> " + QueueID + " and zoneid=" + mZoneId + " order by playorder");
            if (DT.Rows.Count > 0)
            {
                int PlayOrder = 1;
                shell.Instance.ExecuteSQL("dto", "Queue", "Update media.dbo.queue set playorder=" + PlayOrder + " where id=" + QueueID);
                for (int i = 0; i <= DT.Rows.Count - 1; i++)
                {
                    PlayOrder += 1;
                    shell.Instance.ExecuteSQL("dto", "Queue", "Update media.dbo.queue set playorder=" + PlayOrder + " where id=" + DT.Rows[i]["Id"]);
                }
                if (OnNextTrackChanged != null)
                {
                    OnNextTrackChanged(QueueID);
                }
                Processed = true;
            }
            return Processed;
        }

        public bool MoveUp(string QueueID)
        {
            bool Processed = true;

            string NextID = shell.Instance.ExecuteLookUp("dto", "Queue", "Select top 1 id from media.dbo.queue where state=" + QueueStates.Ready + " and deleted=0 and zoneid=" + mZoneId + " order by playorder Asc");
            string ThisPlayOrder = shell.Instance.ExecuteLookUp("dto", "Queue", "Select playorder from media.dbo.queue where id=" + QueueID);
            string PrevID = shell.Instance.ExecuteLookUp("dto", "Queue", "Select top 1 id from media.dbo.queue where state=" + QueueStates.Ready + " and deleted=0 and playorder<" + ThisPlayOrder + " and zoneid=" + mZoneId + " order by playorder desc");
            string PrevPlayOrder = shell.Instance.ExecuteLookUp("dto", "Queue", "Select playorder from media.dbo.queue where id=" + PrevID);

            shell.Instance.ExecuteSQL("dto", "Queue", "Update media.dbo.queue set playorder=" + ThisPlayOrder + " where id=" + PrevID);

            if (PrevID == NextID)
            {
                if (OnNextTrackChanged != null)
                {
                    OnNextTrackChanged(QueueID);
                }
            }
            else
            {
                if (OnChanged != null)
                {
                    OnChanged();
                }
            }
            return Processed;

        }


        public bool MoveDown(string QueueID)
        {
            bool Processed = false;
            string NextID = shell.Instance.ExecuteLookUp("dto", "Queue", "Select top 1 id from media.dbo.queue where state=" + QueueStates.Ready + " and deleted=0 and zoneid=" + mZoneId + " order by playorder Asc");
            string ThisPlayOrder = shell.Instance.ExecuteLookUp("dto", "Queue", "Select playorder from media.dbo.queue where id=" + QueueID);
            string PrevID = shell.Instance.ExecuteLookUp("dto", "Queue", "Select top 1 id from media.dbo.queue where state=" + QueueStates.Ready + " and deleted=0 and playorder>" + ThisPlayOrder + " and zoneid=" + mZoneId + " order by playorder Asc");
            if ( helpers.ToInt(PrevID) > 0)
            {
                string PrevPlayOrder = shell.Instance.ExecuteLookUp("dto", "Queue", "Select playorder from media.dbo.queue where id=" + PrevID);
                shell.Instance.ExecuteSQL("dto", "Queue", "Update media.dbo.queue set playorder=" + ThisPlayOrder + " where id=" + PrevID);
                shell.Instance.ExecuteSQL("dto", "Queue", "Update media.dbo.queue set playorder=" + PrevPlayOrder + " where id=" + QueueID);
                Processed = true;
            }

            if (Processed)
                if (OnChanged != null)
                {
                    OnChanged();
                }

            return Processed;

        }

        public DataTable shuffleTable(DataTable inputTable, int shuffleIterations)
        {
            int index = 0;
            // Remove and throw to the end random rows until we have done so n*3 times (shuffles the dataset)
            for (int i = 0; i <= shuffleIterations; i++)
            {
                index = mRand.Next(0, inputTable.Rows.Count - 1);
                inputTable.Rows.Add(inputTable.Rows[index].ItemArray);
                inputTable.Rows.RemoveAt(index);
            }
            return inputTable;
        }

        public void ShuffleQ()
        {
            try
            {
                DataTable DT = shell.Instance.GetDataTable("dto", "Queue", "Select Id from media.dbo.Queue where deleted=0 and zoneid=" + mZoneId + " and Area='" + mArea + "' and qtype='" + mQueueType + "' and state=" + QueueStates.Ready + " order by playorder asc");
                DT = shuffleTable(DT, DT.Rows.Count * 3);
                int PlayOrder = 0;
                for (int i = 0; i <= DT.Rows.Count - 1; i++)
                {
                    shell.Instance.ExecuteLookUp("dto", "Queue", "Update media.dbo.queue set playorder=" + PlayOrder + " where id=" + DT.Rows[i]["Id"].ToString());
                    PlayOrder += 1;
                }
            }
            catch (Exception ex)
            {
            }

        }


        public void OrderByLastPlay()
        {
            try
            {
                DataTable DT = shell.Instance.GetDataTable("dto", "Queue", "Select Id,PlayedOn from media.dbo.Queue where deleted=0 and zoneid=" + mZoneId + " and Area='" + mArea + "' and qtype='" + mQueueType + "' and state=" + QueueStates.Ready + " order by PlayedOn, PlayOrder");
                int PlayOrder = 0;
                if (DT.Rows.Count > 0)
                {
                    if ( helpers.IsDate(DT.Rows[0]["PlayedOn"].ToString()))
                    {
                        shell.Instance.ExecuteLookUp("dto", "Queue", "update media.dbo.Queue set PlayedOn=NULL where deleted=0 and zoneid=" + mZoneId + " and Area='" + mArea + "' and qtype='" + mQueueType + "' and state=" + QueueStates.Ready);
                        DT = shell.Instance.GetDataTable("dto", "Queue", "Select Id,PlayedOn from media.dbo.Queue where deleted=0 and zoneid=" + mZoneId + " and Area='" + mArea + "' and qtype='" + mQueueType + "' and state=" + QueueStates.Ready + " order by PlayedOn, PlayOrder");
                    }
                    for (int i = 0; i <= DT.Rows.Count - 1; i++)
                    {
                        shell.Instance.ExecuteLookUp("dto", "Queue", "Update media.dbo.queue set playorder=" + PlayOrder + " where id=" + DT.Rows[i]["Id"].ToString());
                        PlayOrder += 1;
                    }
                }
            }
            catch (Exception ex)
            {
            }

        }


        public void ReOrderQueue(string QueueId)
        {
            DataTable DT = shell.Instance.GetDataTable("dto", "pMusicA", "Select Id from media.dbo.queue where state=" + QueueStates.Ready + " and deleted=0 and id <> " + QueueId + " and zoneid=" + mZoneId + " order by playorder");
            if (DT.Rows.Count > 0)
            {
                int PlayOrder = 0;
                for (int i = 0; i <= DT.Rows.Count - 1; i++)
                {
                    PlayOrder += 1;
                    shell.Instance.ExecuteSQL("dto", "pMusicA", "Update media.dbo.queue set playorder=" + PlayOrder + " where id=" + DT.Rows[i]["Id"]);
                }
            }
            DT.Dispose();
            DT = null;

        }




        public bool LoadDirectory(string DirectoryType, string LoadID, string XMLFile, bool Reload, int Duration)
        {

            if (_LoadDirectory(DirectoryType, LoadID, XMLFile, Reload, Duration))
            {
                return true;
            }
            else
            {
                if (_CreateDirectoryFile(DirectoryType, XMLFile))
                {
                    return _LoadDirectory(DirectoryType, LoadID, XMLFile, Reload, Duration);
                }
            }

            return false;
        }


        private bool _LoadDirectory(string DirectoryType, string LoadID, string XMLFile, bool Reload, int Duration)
        {
            //Try
            //    Dim MedPath As String = Path.GetDirectoryName(XMLFile)
            //    If Directory.Exists(MedPath) Then
            //        Dim HasSome As Boolean = False

            //        If File.Exists(XMLFile) Then
            //            Dim DS As DataSet = New DataSet
            //            Dim XMLReader As XmlReadMode
            //            Try
            //                XMLReader = DS.ReadXml(XMLFile)
            //                If DS.Tables.Count > 0 Then
            //                    Try
            //                        Dim DT As DataTable = DS.Tables(0)
            //                        For i As Integer = 0 To DT.Rows.Count - 1
            //                            Try
            //                                Dim Page As String = DT.Rows(i).Item("filename")
            //                                If File.Exists(Page) Then
            //                                    HasSome = True
            //                                    Dim Description As String = Path.GetFileNameWithoutExtension(Page)
            //                                    Select Case DirectoryType
            //                                        Case "Smart"
            //                                            'AddAdvert(LoadID, Page, DirectoryType, Description, Duration)
            //                                        Case "Picture"
            //                                            AddAdvert(LoadID, Page, DirectoryType, Description, Duration)
            //                                        Case "Media"
            //                                            AddAdvert(LoadID, Page, DirectoryType, Description, Duration)
            //                                        Case Else
            //                                            Select Case Path.GetExtension(Page).ToUpper
            //                                                Case ".SWF"
            //                                                    AddAdvert(LoadID, Page, "Flash", Description, Duration)
            //                                                Case ".DLL"
            //                                                    AddAdvert(LoadID, Page, "Smart", Description, Duration)
            //                                                Case ".MPG", ".DAT", ".AVI", ".WMV"
            //                                                    AddAdvert(LoadID, Page, "Media", Description, Duration)
            //                                                Case ".JPG", ".BMP", ".GIF", ".TIF", ".PNG"
            //                                                    AddAdvert(LoadID, Page, "Picture", Description, Duration)
            //                                            End Select


            //                                    End Select
            //                                End If
            //                            Catch ex As Exception
            //                            End Try
            //                        Next
            //                    Catch ex As Exception
            //                    End Try
            //                End If
            //            Catch ex As Exception
            //            End Try
            //        End If
            //    End If

            //Catch ex As Exception
            //    RaiseEvent OnStatus("loaddir ", ex.Message)
            //End Try
            return false;
        }

        private bool _CreateDirectoryFile(string DirectoryType, string XMLFile)
        {

            bool FoundSome = false;
            try
            {
                int i = 0;
                string Exts = "";
                try
                {
                    DataSet DS = new DataSet();
                    DataTable DT = new DataTable();
                    DT.Columns.Add(new DataColumn("filename"));
                    DT.Columns.Add(new DataColumn("played"));
                    DS.Tables.Add(DT);

                    switch (DirectoryType)
                    {
                        case "Smart":
                            Exts = "*.DLL";
                            break;
                        case "Picture":
                            Exts = "*.JPG;*.BMP;*.GIF;*.TIF;*.PNG";
                            break;
                        case "Media":
                            Exts = "*.WMA;*.MP3;*.MPG;*.DAT;*.AVI;*.WMV;*.SWF;*.MP4;*.MOV";
                            break;
                        default:
                            Exts = "*.JPG;*.BMP;*.GIF;*.TIF;*.PNG;*.MPG;*.DAT;*.AVI;*.WMV;.SWF;*.DLL;*.MP4";
                            break;
                    }

                    try
                    {
                        string MedPath = Path.GetDirectoryName(XMLFile);
                        if (Directory.Exists(MedPath))
                        {
                            DirectoryInfo DirInfo = new DirectoryInfo(MedPath);
                            Array ExtArr = Exts.Split(';');

                            for (i = 0; i <= ExtArr.GetLength(0) - 1; i++)
                            {
                                FileInfo[] FileInfos = DirInfo.GetFiles(ExtArr.GetValue(i).ToString());
                                foreach (FileInfo FileInfo in FileInfos)
                                {
                                    DataRow newRow = DT.NewRow();
                                    newRow["filename"] = Path.Combine(MedPath, FileInfo.Name);
                                    newRow["played"] = 0;
                                    DT.Rows.Add(newRow);
                                    FoundSome = true;
                                }
                            }
                            DS.WriteXml(XMLFile);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                }

            }
            catch (Exception ex)
            {
            }

            return FoundSome;

        }

        #region " Property "

        public string ZoneId
        {
            get { return mZoneId; }
        }
        public string FileName
        {
            get { return mMediaFile; }
        }
        public string QueueID
        {
            get { return mQueueId; }
        }
        public string LastQueueID
        {
            get { return mLastQueueId; }
            set { mLastQueueId = value; }
        }

        public string Track
        {
            get { return mTrack; }
        }

        public string TrackType
        {
            get { return mFileType; }
        }


        public string TrackID
        {
            get { return mTrackId; }
        }
        public string PlayListID
        {
            get { return mPlayListID; }
        }

        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; }
        }
        public string Artist
        {
            get { return mArtist; }
        }
        public string Album
        {
            get { return mAlbum; }
        }
        public string BPM
        {
            get { return mBPM; }
        }
        public int Duration
        {
            get { return mDuration; }
            set { mDuration = value; }
        }
        public int CueIn
        {
            get { return mCueIn; }
            set { mCueIn = value; }
        }
        public int FadeIn
        {
            get { return mFadeIn; }
            set { mFadeIn = value; }
        }
        public int CueOut
        {
            get { return mCueOut; }
            set { mCueOut = value; }
        }
        public int FadeOut
        {
            get { return mFadeOut; }
            set { mFadeOut = value; }
        }
        public string StartAt
        {
            get { return mStartAt; }
        }

        public int FadeVolume
        {
            get { return mFadeVolume; }
            set { mFadeVolume = value; }
        }
        public int DefaultVolume
        {
            get { return mDefaultVolume; }
            set { mDefaultVolume = value; }
        }
        public int Volume
        {
            get { return mVolume; }
            set { mVolume = value; }
        }
        public float Rate
        {
            get { return mRate; }
            set { mRate = value; }
        }

        public bool ItemsAdded
        {
            get { return mItemsAdded; }
            set { mItemsAdded = value; }
        }

        public string State
        {
            set
            {
                Array Arr = value.Split(',');

                mCPU =  helpers.ToInt(Arr.GetValue(0).ToString());
                mCPU = (mCPU == float.NaN ? 0 : mCPU);

                mTotalPhys =  helpers.ToInt(Arr.GetValue(1).ToString());
                mTotalPhys = (mTotalPhys == float.NaN ? 0 : mTotalPhys);

                mAvailPhys =  helpers.ToInt(Arr.GetValue(2).ToString());
                mAvailPhys = (mAvailPhys == float.NaN ? 0 : mAvailPhys);

                mTotalVirt =  helpers.ToInt(Arr.GetValue(3).ToString());
                mTotalVirt = (mTotalVirt == float.NaN ? 0 : mTotalVirt);

                mAvailVirt =  helpers.ToInt(Arr.GetValue(4).ToString());
                mAvailVirt = (mAvailVirt == float.NaN ? 0 : mAvailVirt);
            }
        }

        #endregion

    }
}
