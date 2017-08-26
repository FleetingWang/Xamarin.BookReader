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

namespace Xamarin.BookReader.UI.Activities
{
    
public class ReadPDFActivity : BaseActivity {

    public static void start(Context context, String filePath) {
        Intent intent = new Intent(context, ReadPDFActivity.class);
        intent.SetAction(Intent.ActionView);
        intent.SetData(Uri.FromFile(new File(filePath)));
        context.StartActivity(intent);
    }

    //@Bind(R.id.llPdfRoot)
    LinearLayout llPdfRoot;
    private int startX = 0;
    private int startY = 0;

    @Override
    public int getLayoutId() {
        return Resource.Layout.activity_read_pdf;
    }

    @Override
    protected void setupActivityComponent(AppComponent appComponent) {

    }

    @Override
    public void initToolBar() {
        String filePath = Uri.Decode(Intent.DataString.Replace("file://", ""));
        String fileName = filePath.Substring(filePath.LastIndexOf("/") + 1, filePath.LastIndexOf("."));
        mCommonToolbar.Title = (fileName);
        mCommonToolbar.SetNavigationIcon(Resource.Drawable.ab_back);
    }

    @Override
    public void initDatas() {
        if (Intent.ActionView.Equals(Intent.Action)) {
            String filePath = Uri.Decode(Intent.DataString.Replace("file://", ""));

            PDFViewPager pdfViewPager = new PDFViewPager(this, filePath);
            llPdfRoot.addView(pdfViewPager);
        }
    }

    @Override
    public void configViews() {

    }
}

}