using Newtonsoft.Json;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using Xamarin.BookReader.Bases;
using Xamarin.BookReader.Models.Support;
using Xamarin.BookReader.Utils;

namespace Xamarin.BookReader.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string SettingsKey = "settings_key";
        private static readonly string SettingsDefault = string.Empty;
        #endregion


        public static string GeneralSettings
        {
            get
            {
                return AppSettings.GetValueOrDefault(SettingsKey, SettingsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(SettingsKey, value);
            }
        }

        #region FontSize

        private static string GetFontSizeKey(string bookId) => "fontsize_key_" + bookId;
        private static readonly int FontSizeDefault = 16;

        /// <summary>
        /// 全局生效的阅读字体大小
        /// </summary>
        public static int FontSize
        {
            get => GetFontSize("");
            set => SetFontSize("", value);
        }

        public static int GetFontSize(string bookId)
        {
            return AppSettings.GetValueOrDefault(GetFontSizeKey(bookId), ScreenUtils.dpToPxInt(FontSizeDefault));
        }

        /// <summary>
        /// 保存书籍阅读字体大小
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="fontSize"></param>
        public static void SetFontSize(string bookId, int fontSize)
        {
            AppSettings.AddOrUpdateValue(GetFontSizeKey(bookId), fontSize);
        }

        #endregion

        #region ReadBrightness
        private const string ReadBrightnessKey = "readLightness";

        /// <summary>
        /// 阅读界面屏幕亮度
        /// </summary>
        public static int ReadBrightness
        {
            get
            {
                return AppSettings.GetValueOrDefault(ReadBrightnessKey, (int)ScreenUtils.getScreenBrightness(AppUtils.getAppContext()));
            }
            set
            {
                AppSettings.AddOrUpdateValue(ReadBrightnessKey, value);
            }
        }

        #endregion

        #region ReadProgress

        private static string getChapterKey(string bookId) => "chapter_" + bookId;
        private static string getStartPosKey(string bookId) => "startPos_" + bookId;
        private static string getEndPosKey(string bookId) => "endPos_" + bookId;

        public static void SaveReadProgress(string bookId, int currentChapter, int m_mbBufBeginPos, int m_mbBufEndPos)
        {
            AppSettings.AddOrUpdateValue(getChapterKey(bookId), currentChapter);
            AppSettings.AddOrUpdateValue(getStartPosKey(bookId), m_mbBufBeginPos);
            AppSettings.AddOrUpdateValue(getEndPosKey(bookId), m_mbBufEndPos);
        }

        public static int[] GetReadProgress(string bookId)
        {
            int lastChapter = AppSettings.GetValueOrDefault(getChapterKey(bookId), 1);
            int startPos = AppSettings.GetValueOrDefault(getStartPosKey(bookId), 0);
            int endPos = AppSettings.GetValueOrDefault(getEndPosKey(bookId), 0);

            return new int[] { lastChapter, startPos, endPos };
        }

        public static void RemoveReadProgress(string bookId)
        {
            AppSettings.Remove(getChapterKey(bookId));
            AppSettings.Remove(getStartPosKey(bookId));
            AppSettings.Remove(getEndPosKey(bookId));
        }

        #endregion

        #region BookMark

        private static string getBookMarksKey(string bookId) => "marks_" + bookId;
        public static bool AddBookMark(string bookId, BookMark mark)
        {
            List<BookMark> marks = GetValueOrDefault(getBookMarksKey(bookId), new List<BookMark>());
            foreach(var item in marks)
            {
                if(item.chapter == mark.chapter && item.startPos == mark.startPos)
                {
                    return false;
                }
            }
            marks.Add(mark);
            AddOrUpdateValue(getBookMarksKey(bookId), marks);
            return true;
        }
        public static List<BookMark> GetBookMarks(string bookId)
        {
            return GetValueOrDefault(getBookMarksKey(bookId), new List<BookMark>());
        }

        public static void ClearBookMarks(string bookId)
        {
            AppSettings.Remove(getBookMarksKey(bookId));
        }

        #endregion

        #region ReadTheme

        private const string ReadThemeKey = "readTheme";
        private const int ReadThemeDefault = 3;
        public static int ReadTheme
        {
            get
            {
                return AppSettings.GetValueOrDefault(ReadThemeKey, ReadThemeDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(ReadThemeKey, value);
            }
        }
        #endregion

        #region VolumeFlipEnable

        private const string VolumeFlipEnableKey = "volumeFlip";
        private const bool VolumeFlipEnableDefault = true;
        /// <summary>
        /// 是否可以使用音量键翻页
        /// </summary>
        public static bool VolumeFlipEnable
        {
            get
            {
                return AppSettings.GetValueOrDefault(VolumeFlipEnableKey, VolumeFlipEnableDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(VolumeFlipEnableKey, value);
            }
        }
        #endregion

        #region AutoBrightness

        private const string AutoBrightnessKey = "volumeFlip";
        private const bool AutoBrightnessDefault = true;
        public static bool AutoBrightness
        {
            get
            {
                return AppSettings.GetValueOrDefault(AutoBrightnessKey, AutoBrightnessDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(AutoBrightnessKey, value);
            }
        }
        #endregion

        #region UserChooseSex
        private const string UserChooseSexKey = "userChooseSex";
        private static Constant.Gender UserChooseSexDefault = Constant.Gender.Male;
        /// <summary>
        /// 用户选择性别
        /// </summary>
        public static Constant.Gender UserChooseSex
        {
            get
            {
                var userChooseSexStr = AppSettings.GetValueOrDefault(UserChooseSexKey, UserChooseSexDefault.ToString());
                return (Constant.Gender)Enum.Parse(typeof(Constant.Gender), userChooseSexStr);
            }
            set
            {
                AppSettings.AddOrUpdateValue(UserChooseSexKey, value.ToString());
            }
        }
        public static bool IsUserChooseSex
        {
            get => AppSettings.Contains(UserChooseSexKey);
        }
        #endregion

        #region UserChooseSex
        private const string IsNoneCoverKey = "isNoneCover";
        private static bool IsNoneCoverDefault = false;
        public static bool IsNoneCover
        {
            get
            {
                return AppSettings.GetValueOrDefault(IsNoneCoverKey, IsNoneCoverDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(IsNoneCoverKey, value);
            }
        }
        #endregion

        private static T GetValueOrDefault<T>(string key, T defaultValue) where T:new()
        {
            var json = AppSettings.GetValueOrDefault(key, null);
            if (string.IsNullOrEmpty(json))
            {
                return defaultValue;
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
        }

        private static void AddOrUpdateValue<T>(string key, T value) where T: new()
        {
            var json = JsonConvert.SerializeObject(value);
            AppSettings.AddOrUpdateValue(key, json);
        }

    }
}