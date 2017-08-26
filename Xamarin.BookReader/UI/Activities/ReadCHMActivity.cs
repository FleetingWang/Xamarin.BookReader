using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.BookReader.Bases;
using Uri = Android.Net.Uri;
using Android.Webkit;

namespace Xamarin.BookReader.UI.Activities
{
    
/**
 * @author yuyh.
 * @date 2016/12/19.
 */
public class ReadCHMActivity : BaseActivity {

    public static void start(Context context, String filePath) {
        Intent intent = new Intent(context, typeof(ReadCHMActivity));
        intent.SetAction(Intent.ActionView);
        intent.SetData(Uri.FromFile(new Java.IO.File(filePath)));
        context.StartActivity(intent);
    }

    ////@Bind(R.id.progressBar)
    ProgressBar mProgressBar;
    ////@Bind(R.id.webview)
    WebView mWebView;

    private String chmFileName;
    public String chmFilePath = "", extractPath, md5File;

    private List<String> listSite;
    private List<String> listBookmark;

    public override int getLayoutId() {
        return Resource.Layout.activity_read_chm;
    }

    public override void initToolBar() {
        chmFilePath = Uri.Decode(Intent.DataString.Replace("file://", ""));
        chmFileName = chmFilePath.Substring(chmFilePath.LastIndexOf("/") + 1, chmFilePath.LastIndexOf("."));
        mCommonToolbar.Title = (chmFileName);
        mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
    }

    public override void initDatas() {
        Utils.chm = null;
        listSite = new List<String>();
    }

    @Override
    public void configViews() {
        initVweView();

        initFile();
    }

    private void initVweView() {
        mProgressBar.Max = 100);

        mWebView.getSettings().setJavaScriptEnabled(true);
        mWebView.setWebChromeClient(new WebChromeClient() {
            @Override
            public void onProgressChanged(WebView view, int newProgress) {
                super.onProgressChanged(view, newProgress);
                visible(mProgressBar);
                mProgressBar.Progress = (newProgress);
            }
        });
        mWebView.setWebViewClient(new WebViewClient() {
            @Override
            public void onPageStarted(WebView view, String url, Bitmap favicon) {
                if (!url.startsWith("http") && !url.endsWith(md5File)) {
                    String temp = url.Substring("file://".length());
                    if (!temp.startsWith(extractPath)) {
                        url = "file://" + extractPath + temp;
                    }
                }

                super.onPageStarted(view, url, favicon);
                mProgressBar.Progress = (50);
            }

            @Override
            public void onPageFinished(WebView view, String url) {
                super.onPageFinished(view, url);
                mProgressBar.Progress = (100);
                gone(mProgressBar);
            }

            @Override
            public void onLoadResource(WebView view, String url) {
                if (!url.startsWith("http") && !url.endsWith(md5File)) {
                    String temp = url.Substring("file://".length());
                    if (!temp.startsWith(extractPath)) {
                        url = "file://" + extractPath + temp;
                    }
                }
                super.onLoadResource(view, url);
            }


            @Override
            public WebResourceResponse shouldInterceptRequest(WebView view, WebResourceRequest request) {
                String url = request.getUrl().toString();
                if (!url.startsWith("http") && !url.endsWith(md5File)) {
                    String temp = url.Substring("file://".length());
                    String insideFileName;
                    if (!temp.startsWith(extractPath)) {
                        url = "file://" + extractPath + temp;
                        insideFileName = temp;
                    } else {
                        insideFileName = temp.Substring(extractPath.length());
                    }
                    if (insideFileName.contains("#")) {
                        insideFileName = insideFileName.Substring(0, insideFileName.indexOf("#"));
                    }
                    if (insideFileName.contains("?")) {
                        insideFileName = insideFileName.Substring(0, insideFileName.indexOf("?"));
                    }
                    if (insideFileName.contains("%20")) {
                        insideFileName = insideFileName.replaceAll("%20", " ");
                    }
                    if (url.endsWith(".gif") || url.endsWith(".jpg") || url.endsWith(".png")) {
                        try {
                            return new WebResourceResponse("image/*", "", Utils.chm.getResourceAsStream(insideFileName));
                        } catch (IOException e) {
                            e.printStackTrace();
                            return super.shouldInterceptRequest(view, request);
                        }
                    } else if (url.endsWith(".css") || url.endsWith(".js")) {
                        try {
                            return new WebResourceResponse("", "", Utils.chm.getResourceAsStream(insideFileName));
                        } catch (IOException e) {
                            e.printStackTrace();
                            return super.shouldInterceptRequest(view, request);
                        }
                    } else {
                        Utils.extractSpecificFile(chmFilePath, extractPath + insideFileName, insideFileName);
                    }
                }
                Log.e("2, webviewrequest", url);
                return super.shouldInterceptRequest(view, request);
            }

            @Override
            public bool shouldOverrideUrlLoading(WebView view, String url) {
                if (!url.startsWith("http") && !url.endsWith(md5File)) {
                    String temp = url.Substring("file://".length());
                    if (!temp.startsWith(extractPath)) {
                        url = "file://" + extractPath + temp;
                        view.loadUrl(url);
                        return true;
                    }
                }
                return false;
            }

            public bool shouldOverrideUrlLoading(WebView view, WebResourceRequest request) {
                return shouldOverrideUrlLoading(view, request.getUrl().toString());
            }
        });
        mWebView.getSettings().setBuiltInZoomControls(true);
        mWebView.getSettings().setDisplayZoomControls(false);
        mWebView.getSettings().setUseWideViewPort(true);
        mWebView.getSettings().setLoadWithOverviewMode(true);
        mWebView.getSettings().setLoadsImagesAutomatically(true);
    }

    private void initFile() {
        new AsyncTask<Void, Void, Void>() {
            int historyIndex = 1;

            @Override
            protected void onPreExecute() {
                super.onPreExecute();
                showDialog();
            }

            @Override
            protected Void doInBackground(Void... voids) {
                md5File = Utils.checkSum(chmFilePath);
                extractPath = Constant.PATH_CHM + "/" + md5File;
                if (!(new File(extractPath).exists())) {
                    if (Utils.extract(chmFilePath, extractPath)) {
                        try {
                            listSite = Utils.domparse(chmFilePath, extractPath, md5File);
                            listBookmark = Utils.getBookmark(extractPath, md5File);
                        } catch (IOException e) {
                            e.printStackTrace();
                        }
                    } else {
                        (new File(extractPath)).delete();
                    }
                } else {
                    listSite = Utils.getListSite(extractPath, md5File);
                    listBookmark = Utils.getBookmark(extractPath, md5File);
                    historyIndex = Utils.getHistory(extractPath, md5File);
                }
                return null;
            }

            @Override
            protected void onPostExecute(Void aVoid) {
                super.onPostExecute(aVoid);
                mWebView.loadUrl("file://" + extractPath + "/" + listSite.get(historyIndex));
                hideDialog();
            }
        }.execute();
    }

    @Override
    public bool onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.menu_chm_reader, menu);

        MenuItem searchMenuItem = menu.findItem(R.id.menu_search);//在菜单中找到对应控件的item
        SearchView searchView = (SearchView) MenuItemCompat.getActionView(searchMenuItem);
        searchView.setOnCloseListener(new SearchView.OnCloseListener() {
            @Override
            public bool onClose() {
                mWebView.clearMatches();
                return false;
            }
        });
        searchView.setOnQueryTextListener(new SearchView.OnQueryTextListener() {
            @Override
            public bool onQueryTextSubmit(String query) {
                return false;
            }

            @Override
            public bool onQueryTextChange(String newText) {
                mWebView.findAllAsync(newText);
                try {
                    for (Method m : WebView.class.getDeclaredMethods()) {
                        if (m.getName().Equals("setFindIsUp")) {
                            m.setAccessible(true);
                            m.invoke(mWebView, true);
                            break;
                        }
                    }
                } catch (Exception ignored) {
                }
                return false;
            }
        });
        return true;
    }

    @Override
    public bool onOptionsItemSelected(MenuItem item) {
        return super.onOptionsItemSelected(item);
    }
}

}