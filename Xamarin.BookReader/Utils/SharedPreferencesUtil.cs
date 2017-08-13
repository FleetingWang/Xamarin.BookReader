using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Annotation;
using Java.IO;
using Java.Lang;
using Android.Util;

namespace Xamarin.BookReader.Utils
{
    public class SharedPreferencesUtil
    {
        private static SharedPreferencesUtil prefsUtil;
        public Context context;
        public ISharedPreferences prefs;
        public ISharedPreferencesEditor editor;

        public /*synchronized*/ static SharedPreferencesUtil getInstance()
        {
            return prefsUtil;
        }

        public static void init(Context context, string prefsname, FileCreationMode mode)
        {
            prefsUtil = new SharedPreferencesUtil();
            prefsUtil.context = context;
            prefsUtil.prefs = prefsUtil.context.GetSharedPreferences(prefsname, mode);
            prefsUtil.editor = prefsUtil.prefs.Edit();
        }

        private SharedPreferencesUtil()
        {
        }


        public bool getBoolean(string key, bool defaultVal)
        {
            return this.prefs.GetBoolean(key, defaultVal);
        }

        public bool getBoolean(string key)
        {
            return this.prefs.GetBoolean(key, false);
        }


        public string getString(string key, string defaultVal)
        {
            return this.prefs.GetString(key, defaultVal);
        }

        public string getString(string key)
        {
            return this.prefs.GetString(key, null);
        }

        public int getInt(string key, int defaultVal)
        {
            return this.prefs.GetInt(key, defaultVal);
        }

        public int getInt(string key)
        {
            return this.prefs.GetInt(key, 0);
        }


        public float getFloat(string key, float defaultVal)
        {
            return this.prefs.GetFloat(key, defaultVal);
        }

        public float getFloat(string key)
        {
            return this.prefs.GetFloat(key, 0f);
        }

        public long getLong(string key, long defaultVal)
        {
            return this.prefs.GetLong(key, defaultVal);
        }

        public long getLong(string key)
        {
            return this.prefs.GetLong(key, 0l);
        }

        [TargetApi(Value = (int)BuildVersionCodes.Honeycomb)]
        public ICollection<string> getStringSet(string key, ICollection<string> defaultVal)
        {
            return this.prefs.GetStringSet(key, defaultVal);
        }

        [TargetApi(Value = (int)BuildVersionCodes.Honeycomb)]
        public ICollection<string> getStringSet(string key)
        {
            return this.prefs.GetStringSet(key, null);
        }

        public IDictionary<string,object> getAll()
        {
            return this.prefs.All;
        }

        public bool exists(string key)
        {
            return prefs.Contains(key);
        }


        public SharedPreferencesUtil putString(string key, string value)
        {
            editor.PutString(key, value);
            editor.Commit();
            return this;
        }

        public SharedPreferencesUtil putInt(string key, int value)
        {
            editor.PutInt(key, value);
            editor.Commit();
            return this;
        }

        public SharedPreferencesUtil putFloat(string key, float value)
        {
            editor.PutFloat(key, value);
            editor.Commit();
            return this;
        }

        public SharedPreferencesUtil putLong(string key, long value)
        {
            editor.PutLong(key, value);
            editor.Commit();
            return this;
        }

        public SharedPreferencesUtil putBoolean(string key, bool value)
        {
            editor.PutBoolean(key, value);
            editor.Commit();
            return this;
        }

        public void commit()
        {
            editor.Commit();
        }

        [TargetApi(Value = (int)BuildVersionCodes.Honeycomb)]
        public SharedPreferencesUtil putStringSet(string key, ICollection<string> value)
        {
            editor.PutStringSet(key, value);
            editor.Commit();
            return this;
        }

        public void putObject(string key, Object obj)
        {
            System.IO.MemoryStream baos = new System.IO.MemoryStream();
            ObjectOutputStream outStream = null;
            try
            {
                outStream = new ObjectOutputStream(baos);
                outStream.WriteObject(obj);
                string objectVal = ASCIIEncoding.ASCII.GetString(Base64.Encode(baos.ToArray(), Base64Flags.Default));
                editor.PutString(key, objectVal);
                editor.Commit();
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
            }
            finally
            {
                try
                {
                    if (baos != null)
                    {
                        baos.Close();
                    }
                    if (outStream != null) {
                        outStream.Close();
                    }
                }
                catch (IOException e)
                {
                    e.PrintStackTrace();
                }
            }
        }

        public T getObject<T>(string key) where T:Java.Lang.Object
        {
            if (prefs.Contains(key))
            {
                string objectVal = prefs.GetString(key, null);
                byte[] buffer = Base64.Decode(objectVal, Base64Flags.Default);
                System.IO.MemoryStream bais = new System.IO.MemoryStream(buffer);
                ObjectInputStream ois = null;
                try
                {
                    ois = new ObjectInputStream(bais);
                    T t = (T)ois.ReadObject();
                    return t;
                }
                catch (StreamCorruptedException e)
                {
                    e.PrintStackTrace();
                }
                catch (IOException e)
                {
                    e.PrintStackTrace();
                }
                catch (ClassNotFoundException e)
                {
                    e.PrintStackTrace();
                }
                finally
                {
                    try
                    {
                        if (bais != null)
                        {
                            bais.Close();
                        }
                        if (ois != null)
                        {
                            ois.Close();
                        }
                    }
                    catch (IOException e)
                    {
                        e.PrintStackTrace();
                    }
                }
            }
            return null;
        }

        public SharedPreferencesUtil remove(string key)
        {
            editor.Remove(key);
            editor.Commit();
            return this;
        }

        public SharedPreferencesUtil removeAll()
        {
            editor.Clear();
            editor.Commit();
            return this;
        }
    }
}