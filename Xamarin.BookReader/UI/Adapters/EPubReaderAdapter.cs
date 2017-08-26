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

namespace Xamarin.BookReader.UI.Adapters
{
    
public class EPubReaderAdapter : FragmentPagerAdapter {

    private List<SpineReference> mSpineReferences;
    private Book mBook;
    private String mEpubFileName;
    private bool mIsSmilAvailable;

    public EPubReaderAdapter(FragmentManager fm, List<SpineReference> spineReferences,
                             Book book, String epubFilename, bool isSmilAvilable) {
        super(fm);
        this.mSpineReferences = spineReferences;
        this.mBook = book;
        this.mEpubFileName = epubFilename;
        this.mIsSmilAvailable = isSmilAvilable;
    }

    @Override
    public Fragment getItem(int position) {
        return EPubReaderFragment.newInstance(position, mBook, mEpubFileName, mIsSmilAvailable);
    }

    @Override
    public int getCount() {
        return mSpineReferences.size();
    }

}
}