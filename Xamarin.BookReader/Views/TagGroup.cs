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

using Android.Content.Res;
using Android.Graphics;
using Android.Text;
using Android.Util;
using Android.Text.Method;
using Xamarin.BookReader.Views;
using static Android.Graphics.Paint;
using Android.Views.InputMethods;
using Java.Lang;

namespace Xamarin.BookReader.Views
{
    public class TagGroup : ViewGroup
    {
        private int default_border_color = Color.Rgb(0x49, 0xC1, 0x20);
        private int default_text_color = Color.Rgb(0x49, 0xC1, 0x20);
        private int default_background_color = Color.White;
        private int default_dash_border_color = Color.Rgb(0xAA, 0xAA, 0xAA);
        private int default_input_hint_color = Color.Argb(0x80, 0x00, 0x00, 0x00);
        private int default_input_text_color = Color.Argb(0xDE, 0x00, 0x00, 0x00);
        private int default_checked_border_color = Color.Rgb(0x49, 0xC1, 0x20);
        private int default_checked_text_color = Color.White;
        private int default_checked_marker_color = Color.White;
        private int default_checked_background_color = Color.Rgb(0x49, 0xC1, 0x20);
        private int default_pressed_background_color = Color.Rgb(0xED, 0xED, 0xED);
        private float default_border_stroke_width;
        private float default_text_size;
        private float default_horizontal_spacing;
        private float default_vertical_spacing;
        private float default_horizontal_padding;
        private float default_vertical_padding;

        /** Indicates whether this TagGroup is set up to APPEND mode or DISPLAY mode. Default is false. */
        private bool isAppendMode;

        /** The text to be displayed when the text of the INPUT tag is empty. */
        private string inputHint;

        /** The tag outline border color. */
        private int borderColor;

        /** The tag text color. */
        private int textColor;

        /** The tag background color. */
        private int backgroundColor;

        /** The dash outline border color. */
        private int dashBorderColor;

        /** The  input tag hint text color. */
        private int inputHintColor;

        /** The input tag type text color. */
        private int inputTextColor;

        /** The checked tag outline border color. */
        private int checkedBorderColor;

        /** The check text color */
        private int checkedTextColor;

        /** The checked marker color. */
        private int checkedMarkerColor;

        /** The checked tag background color. */
        private int checkedBackgroundColor;

        /** The tag background color, when the tag is being pressed. */
        private int pressedBackgroundColor;

        /** The tag outline border stroke width, default is 0.5dp. */
        private float borderStrokeWidth;

        /** The tag text size, default is 13sp. */
        private float textSize;

        /** The horizontal tag spacing, default is 8.0dp. */
        private int horizontalSpacing;

        /** The vertical tag spacing, default is 4.0dp. */
        private int verticalSpacing;

        /** The horizontal tag padding, default is 12.0dp. */
        private int horizontalPadding;

        /** The vertical tag padding, default is 3.0dp. */
        private int verticalPadding;

        /** Listener used to dispatch tag change event. */
        private OnTagChangeListener mOnTagChangeListener;

        /** Listener used to dispatch tag click event. */
        private OnTagClickListener mOnTagClickListener;

        public TagGroup(Context context) : this(context, null)
        {

        }

        public TagGroup(Context context, IAttributeSet attrs) : this(context, attrs, Resource.Attribute.tagGroupStyle)
        {

        }

        public TagGroup(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {

            default_border_stroke_width = dp2px(0.5f);
            default_text_size = sp2px(13.0f);
            default_horizontal_spacing = dp2px(8.0f);
            default_vertical_spacing = dp2px(4.0f);
            default_horizontal_padding = dp2px(12.0f);
            default_vertical_padding = dp2px(3.0f);

            // Load styled attributes.
            TypedArray a = context.ObtainStyledAttributes(attrs, Resource.Styleable.TagGroup, defStyleAttr, Resource.Style.TagGroup);
            try
            {
                isAppendMode = a.GetBoolean(Resource.Styleable.TagGroup_atg_isAppendMode, false);
                inputHint = a.GetText(Resource.Styleable.TagGroup_atg_inputHint);
                borderColor = a.GetColor(Resource.Styleable.TagGroup_atg_borderColor, default_border_color);
                textColor = a.GetColor(Resource.Styleable.TagGroup_atg_textColor, default_text_color);
                backgroundColor = a.GetColor(Resource.Styleable.TagGroup_atg_backgroundColor, default_background_color);
                dashBorderColor = a.GetColor(Resource.Styleable.TagGroup_atg_dashBorderColor, default_dash_border_color);
                inputHintColor = a.GetColor(Resource.Styleable.TagGroup_atg_inputHintColor, default_input_hint_color);
                inputTextColor = a.GetColor(Resource.Styleable.TagGroup_atg_inputTextColor, default_input_text_color);
                checkedBorderColor = a.GetColor(Resource.Styleable.TagGroup_atg_checkedBorderColor, default_checked_border_color);
                checkedTextColor = a.GetColor(Resource.Styleable.TagGroup_atg_checkedTextColor, default_checked_text_color);
                checkedMarkerColor = a.GetColor(Resource.Styleable.TagGroup_atg_checkedMarkerColor, default_checked_marker_color);
                checkedBackgroundColor = a.GetColor(Resource.Styleable.TagGroup_atg_checkedBackgroundColor, default_checked_background_color);
                pressedBackgroundColor = a.GetColor(Resource.Styleable.TagGroup_atg_pressedBackgroundColor, default_pressed_background_color);
                borderStrokeWidth = a.GetDimension(Resource.Styleable.TagGroup_atg_borderStrokeWidth, default_border_stroke_width);
                textSize = a.GetDimension(Resource.Styleable.TagGroup_atg_textSize, default_text_size);
                horizontalSpacing = (int)a.GetDimension(Resource.Styleable.TagGroup_atg_horizontalSpacing, default_horizontal_spacing);
                verticalSpacing = (int)a.GetDimension(Resource.Styleable.TagGroup_atg_verticalSpacing, default_vertical_spacing);
                horizontalPadding = (int)a.GetDimension(Resource.Styleable.TagGroup_atg_horizontalPadding, default_horizontal_padding);
                verticalPadding = (int)a.GetDimension(Resource.Styleable.TagGroup_atg_verticalPadding, default_vertical_padding);
            }
            finally
            {
                a.Recycle();
            }

            if (isAppendMode)
            {
                // Append the initial INPUT tag.
                appendInputTag();

                Click += (sender, e) =>
                {
                    submitTag();
                };
            }
        }

        /**
         * Call this to submit the INPUT tag.
         */
        public void submitTag()
        {
            TagView inputTag = getInputTag();
            if (inputTag != null && inputTag.isInputAvailable())
            {
                inputTag.endInput();

                if (mOnTagChangeListener != null)
                {
                    mOnTagChangeListener.onAppend(this, inputTag.Text);
                }
                appendInputTag();
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            MeasureSpecMode widthMode = MeasureSpec.GetMode(widthMeasureSpec);
            MeasureSpecMode heightMode = MeasureSpec.GetMode(heightMeasureSpec);
            int widthSize = MeasureSpec.GetSize(widthMeasureSpec);
            int heightSize = MeasureSpec.GetSize(heightMeasureSpec);

            MeasureChildren(widthMeasureSpec, heightMeasureSpec);

            int width = 0;
            int height = 0;

            int row = 0; // The row counter.
            int rowWidth = 0; // Calc the current row width.
            int rowMaxHeight = 0; // Calc the max tag height, in current row.

            int count = ChildCount;
            for (int i = 0; i < count; i++)
            {
                View child = GetChildAt(i);
                int childWidth = child.MeasuredWidth;
                int childHeight = child.MeasuredHeight;

                if (child.Visibility != ViewStates.Gone)
                {
                    rowWidth += childWidth;
                    if (rowWidth > widthSize)
                    { // Next line.
                        rowWidth = childWidth; // The next row width.
                        height += rowMaxHeight + verticalSpacing;
                        rowMaxHeight = childHeight; // The next row max height.
                        row++;
                    }
                    else
                    { // This line.
                        rowMaxHeight = System.Math.Max(rowMaxHeight, childHeight);
                    }
                    rowWidth += horizontalSpacing;
                }
            }
            // Account for the last row height.
            height += rowMaxHeight;

            // Account for the padding too.
            height += PaddingTop + PaddingBottom;

            // If the tags grouped in one row, set the width to wrap the tags.
            if (row == 0)
            {
                width = rowWidth;
                width += PaddingLeft + PaddingRight;
            }
            else
            {// If the tags grouped exceed one line, set the width to match the parent.
                width = widthSize;
            }

            SetMeasuredDimension(widthMode == MeasureSpecMode.Exactly ? widthSize : width,
                    heightMode == MeasureSpecMode.Exactly ? heightSize : height);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            int parentLeft = PaddingLeft;
            int parentRight = r - l - PaddingRight;
            int parentTop = PaddingTop;
            int parentBottom = b - t - PaddingBottom;

            int childLeft = parentLeft;
            int childTop = parentTop;

            int rowMaxHeight = 0;

            int count = ChildCount;
            for (int i = 0; i < count; i++)
            {
                View child = GetChildAt(i);
                int width = child.MeasuredWidth;
                int height = child.MeasuredHeight;

                if (child.Visibility != ViewStates.Gone)
                {
                    if (childLeft + width > parentRight)
                    { // Next line
                        childLeft = parentLeft;
                        childTop += rowMaxHeight + verticalSpacing;
                        rowMaxHeight = height;
                    }
                    else
                    {
                        rowMaxHeight = System.Math.Max(rowMaxHeight, height);
                    }
                    child.Layout(childLeft, childTop, childLeft + width, childTop + height);

                    childLeft += width + horizontalSpacing;
                }
            }
        }

        protected override IParcelable OnSaveInstanceState()
        {
            IParcelable superState = base.OnSaveInstanceState();
            SavedState ss = new SavedState(superState);
            ss.tags = getTags();
            ss.checkedPosition = getCheckedTagIndex();
            if (getInputTag() != null)
            {
                ss.input = getInputTag().Text;
            }
            return ss;
        }

        protected override void OnRestoreInstanceState(IParcelable state)
        {
            if (!(state is SavedState))
            {
                base.OnRestoreInstanceState(state);
                return;
            }

            SavedState ss = (SavedState)state;
            base.OnRestoreInstanceState(ss.SuperState);

            setTags(ss.tags);
            TagView checkedTagView = getTagAt(ss.checkedPosition);
            if (checkedTagView != null)
            {
                checkedTagView.setChecked(true);
            }
            if (getInputTag() != null)
            {
                getInputTag().Text = (ss.input);
            }
        }

        /**
         * Returns the INPUT tag view in this group.
         *
         * @return the INPUT state tag view or null if not exists
         */
        protected TagView getInputTag()
        {
            if (isAppendMode)
            {
                int inputTagIndex = ChildCount - 1;
                TagView inputTag = getTagAt(inputTagIndex);
                if (inputTag != null && inputTag.mState == TagView.STATE_INPUT)
                {
                    return inputTag;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /**
         * Returns the INPUT state tag in this group.
         *
         * @return the INPUT state tag view or null if not exists
         */
        public string getInputTagText()
        {
            TagView inputTagView = getInputTag();
            if (inputTagView != null)
            {
                return inputTagView.Text;
            }
            return null;
        }

        /**
         * Return the last NORMAL state tag view in this group.
         *
         * @return the last NORMAL state tag view or null if not exists
         */
        protected TagView getLastNormalTagView()
        {
            int lastNormalTagIndex = isAppendMode ? ChildCount - 2 : ChildCount - 1;
            TagView lastNormalTagView = getTagAt(lastNormalTagIndex);
            return lastNormalTagView;
        }

        /**
         * Returns the tag array in group, except the INPUT tag.
         *
         * @return the tag array.
         */
        public string[] getTags()
        {
            int count = ChildCount;
            List<string> tagList = new List<string>();
            for (int i = 0; i < count; i++)
            {
                TagView tagView = getTagAt(i);
                if (tagView.mState == TagView.STATE_NORMAL)
                {
                    tagList.Add(tagView.Text);
                }
            }

            return tagList.ToArray();
        }

        /**
         * @see #setTags(String...)
         */
        public void setTags(List<string> tagList)
        {
            setTags(tagList.ToArray());
        }

        /**
         * Set the tags. It will remove all previous tags first.
         *
         * @param tags the tag list to set.
         */
        public void setTags(params string[] tags)
        {
            setTags(null, tags);
        }

        public void setTags(List<TagColor> colors, params string[] tags)
        {
            RemoveAllViews();
            int i = 0;
            foreach (string tag in tags)
            {
                TagColor color = null;
                if (colors != null)
                {
                    color = colors[i++];
                }
                appendTag(color, tag);
            }

            if (isAppendMode)
            {
                appendInputTag();
            }
        }

        /**
         * Returns the tag view at the specified position in the group.
         *
         * @param index the position at which to get the tag view from.
         * @return the tag view at the specified position or null if the position
         * does not exists within this group.
         */
        protected TagView getTagAt(int index)
        {
            return (TagView)GetChildAt(index);
        }

        /**
         * Returns the checked tag view in the group.
         *
         * @return the checked tag view or null if not exists.
         */
        protected TagView getCheckedTag()
        {
            int checkedTagIndex = getCheckedTagIndex();
            if (checkedTagIndex != -1)
            {
                return getTagAt(checkedTagIndex);
            }
            return null;
        }

        /**
         * Return the checked tag index.
         *
         * @return the checked tag index, or -1 if not exists.
         */
        protected int getCheckedTagIndex()
        {
            int count = ChildCount;
            for (int i = 0; i < count; i++)
            {
                TagView tag = getTagAt(i);
                if (tag.isChecked)
                {
                    return i;
                }
            }
            return -1;
        }

        /**
         * Register a callback to be invoked when this tag group is changed.
         *
         * @param l the callback that will run
         */
        public void setOnTagChangeListener(OnTagChangeListener l)
        {
            mOnTagChangeListener = l;
        }

        /**
         * @see #appendInputTag(String)
         */
        protected void appendInputTag()
        {
            appendInputTag(null);
        }

        /**
         * Append a INPUT tag to this group. It will throw an exception if there has a previous INPUT tag.
         *
         * @param tag the tag text.
         */
        protected void appendInputTag(string tag)
        {
            appendInputTag(null, tag);
        }

        protected void appendInputTag(TagColor color, string tag)
        {
            TagView previousInputTag = getInputTag();
            if (previousInputTag != null)
            {
                throw new IllegalStateException("Already has a INPUT tag in group.");
            }

            TagView newInputTag = new TagView(this, Context, TagView.STATE_INPUT, color, tag);
            newInputTag.Click += NewTag_Click;

            AddView(newInputTag);
        }

        /**
         * Append tag to this group.
         *
         * @param tag the tag to append.
         */
        protected void appendTag(TagColor color, string tag)
        {
            TagView newTag = new TagView(this, Context, TagView.STATE_NORMAL, color, tag);
            newTag.Click += NewTag_Click;
            AddView(newTag);
        }

        private void NewTag_Click(object sender, EventArgs e)
        {
            TagView tag = (TagView)sender;
            if (isAppendMode)
            {
                if (tag.mState == TagView.STATE_INPUT)
                {
                    // If the clicked tag is in INPUT state, uncheck the previous checked tag if exists.
                    TagView checkedTag = getCheckedTag();
                    if (checkedTag != null)
                    {
                        checkedTag.setChecked(false);
                    }
                }
                else
                {
                    // If the clicked tag is currently checked, delete the tag.
                    if (tag.isChecked)
                    {
                        deleteTag(tag);
                    }
                    else
                    {
                        // If the clicked tag is unchecked, uncheck the previous checked tag if exists,
                        // then check the clicked tag.
                        TagView checkedTag = getCheckedTag();
                        if (checkedTag != null)
                        {
                            checkedTag.setChecked(false);
                        }
                        tag.setChecked(true);
                    }
                }
            }
            else
            {
                if (mOnTagClickListener != null)
                {
                    mOnTagClickListener.onTagClick(tag.Text);
                }
            }
        }

        public float dp2px(float dp)
        {
            return TypedValue.ApplyDimension(ComplexUnitType.Dip, dp,
                    Resources.DisplayMetrics);
        }

        public float sp2px(float sp)
        {
            return TypedValue.ApplyDimension(ComplexUnitType.Sp, sp,
                    Resources.DisplayMetrics);
        }

        public override ViewGroup.LayoutParams GenerateLayoutParams(IAttributeSet attrs)
        {
            return new TagGroup.LayoutParams(Context, attrs);
        }

        /**
         * Register a callback to be invoked when a tag is clicked.
         *
         * @param l the callback that will run.
         */
        public void setOnTagClickListener(OnTagClickListener l)
        {
            mOnTagClickListener = l;
        }

        protected void deleteTag(TagView tagView)
        {
            RemoveView(tagView);
            if (mOnTagChangeListener != null)
            {
                mOnTagChangeListener.onDelete(this, tagView.Text);
            }
        }

        /**
         * Interface definition for a callback to be invoked when a tag group is changed.
         */
        public interface OnTagChangeListener
        {
            /**
             * Called when a tag has been appended to the group.
             *
             * @param tag the appended tag.
             */
            void onAppend(TagGroup tagGroup, string tag);

            /**
             * Called when a tag has been deleted from the the group.
             *
             * @param tag the deleted tag.
             */
            void onDelete(TagGroup tagGroup, string tag);
        }

        /**
         * Interface definition for a callback to be invoked when a tag is clicked.
         */
        public interface OnTagClickListener
        {
            /**
             * Called when a tag has been clicked.
             *
             * @param tag The tag text of the tag that was clicked.
             */
            void onTagClick(string tag);
        }

        /**
         * Per-child layout information for layouts.
         */
        public class LayoutParams : ViewGroup.LayoutParams
        {
            public LayoutParams(Context c, IAttributeSet attrs) : base(c, attrs)
            {

            }

            public LayoutParams(int width, int height) : base(width, height)
            {

            }
        }

        /**
         * For {@link TagGroup} save and restore state.
         */
        class SavedState : BaseSavedState
        {
            // TODO: Parcelable.Creator
            //public static Parcelable.Creator<SavedState> CREATOR =
            //        new Parcelable.Creator<SavedState>() {
            //            public SavedState createFromParcel(Parcel in) {
            //                return new SavedState(in);
            //            }

            //            public SavedState[] newArray(int size) {
            //                return new SavedState[size];
            //            }
            //        };
            int tagCount;
            public string[] tags;
            public int checkedPosition;
            public string input;

            public SavedState(Parcel source) : base(source)
            {

                tagCount = source.ReadInt();
                tags = new string[tagCount];
                source.ReadStringArray(tags);
                checkedPosition = source.ReadInt();
                input = source.ReadString();
            }

            public SavedState(IParcelable superState) : base(superState)
            {

            }

            public override void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
            {
                base.WriteToParcel(dest, flags);
                tagCount = tags.Length;
                dest.WriteInt(tagCount);
                dest.WriteStringArray(tags);
                dest.WriteInt(checkedPosition);
                dest.WriteString(input);
            }
        }


        /**
         * The tag view which has two states can be either NORMAL or INPUT.
         */
        protected class TagView : TextView
        {



            public static int STATE_NORMAL = 1;
            public static int STATE_INPUT = 2;

            /** The offset to the text. */
            private static int CHECKED_MARKER_OFFSET = 3;

            /** The stroke width of the checked marker */
            private static int CHECKED_MARKER_STROKE_WIDTH = 4;

            /** The current state. */
            public int mState;

            /** Indicates the tag if checked. */
            public bool isChecked = false;

            /** Indicates the tag if pressed. */
            private bool isPressed = false;

            private Paint mBorderPaint = new Paint { Flags = PaintFlags.AntiAlias };

            private Paint mBackgroundPaint = new Paint { Flags = PaintFlags.AntiAlias };

            private Paint mCheckedMarkerPaint = new Paint { Flags = PaintFlags.AntiAlias };

            /** The rect for the tag's left corner drawing. */
            private RectF mLeftCornerRectF = new RectF();

            /** The rect for the tag's right corner drawing. */
            private RectF mRightCornerRectF = new RectF();

            /** The rect for the tag's horizontal blank fill area. */
            private RectF mHorizontalBlankFillRectF = new RectF();

            /** The rect for the tag's vertical blank fill area. */
            private RectF mVerticalBlankFillRectF = new RectF();

            /** The rect for the checked mark draw bound. */
            private RectF mCheckedMarkerBound = new RectF();

            /** Used to detect the touch event. */
            private Rect mOutRect = new Rect();

            /** The path for draw the tag's outline border. */
            private Path mBorderPath = new Path();

            /** The path effect provide draw the dash border. */
            private PathEffect mPathEffect = new DashPathEffect(new float[] { 10, 5 }, 0);

            private TagColor color;
            private TagGroup tagGroup;
            public TagView(TagGroup tagGroup, Context context, int state, TagColor color, string text) : base(context)
            {
                this.tagGroup = tagGroup;
                this.color = color;
                SetPadding(tagGroup.horizontalPadding, tagGroup.verticalPadding, tagGroup.horizontalPadding, tagGroup.verticalPadding);
                LayoutParameters = new TagGroup.LayoutParams(
                        TagGroup.LayoutParams.WrapContent,
                        TagGroup.LayoutParams.WrapContent);

                Gravity = (GravityFlags.Center);
                Text = text;
                SetTextSize(ComplexUnitType.Px, tagGroup.textSize);

                mState = state;

                Clickable = (tagGroup.isAppendMode);
                Focusable = (state == STATE_INPUT);
                FocusableInTouchMode = (state == STATE_INPUT);
                Hint = (state == STATE_INPUT ? tagGroup.inputHint : null);
                MovementMethod = (state == STATE_INPUT ? ArrowKeyMovementMethod.Instance : null);

                mBorderPaint.SetStyle(Style.Stroke);
                mBorderPaint.StrokeWidth = tagGroup.borderStrokeWidth;
                mBackgroundPaint.SetStyle(Style.Fill);
                mCheckedMarkerPaint.SetStyle(Style.Fill);
                mCheckedMarkerPaint.StrokeWidth = CHECKED_MARKER_STROKE_WIDTH;
                mCheckedMarkerPaint.Color = new Color(tagGroup.checkedMarkerColor);

                // Interrupted long click event to avoid PAUSE popup.
                //setOnLongClickListener(new OnLongClickListener() {
                //    @Override
                //    public bool onLongClick(View v) {
                //        return state != STATE_INPUT;
                //    }
                //});

                if (state == STATE_INPUT)
                {
                    RequestFocus();

                    // Handle the ENTER key down.
                    //setOnEditorActionListener(new OnEditorActionListener() {
                    //    @Override
                    //    public bool onEditorAction(TextView v, int actionId, KeyEvent event) {
                    //        if (actionId == EditorInfo.IME_NULL
                    //                && (event != null && event.getKeyCode() == KeyEvent.KEYCODE_ENTER
                    //                && event.getAction() == KeyEvent.ACTION_DOWN)) {
                    //            if (isInputAvailable()) {
                    //                // If the input content is available, end the input and dispatch
                    //                // the event, then append a new INPUT state tag.
                    //                endInput();
                    //                if (mOnTagChangeListener != null) {
                    //                    mOnTagChangeListener.onAppend(TagGroup.this, getText().ToString());
                    //                }
                    //                appendInputTag();
                    //            }
                    //            return true;
                    //        }
                    //        return false;
                    //    }
                    //});

                    // Handle the BACKSPACE key down.
                    //setOnKeyListener(new OnKeyListener() {
                    //    @Override
                    //    public bool onKey(View v, int keyCode, KeyEvent event) {
                    //        if (keyCode == Keycode.Del && event.getAction() == KeyEvent.ACTION_DOWN) {
                    //            // If the input content is empty, check or remove the last NORMAL state tag.
                    //            if (TextUtils.isEmpty(getText().ToString())) {
                    //                TagView lastNormalTagView = getLastNormalTagView();
                    //                if (lastNormalTagView != null) {
                    //                    if (lastNormalTagView.isChecked) {
                    //                        removeView(lastNormalTagView);
                    //                        if (mOnTagChangeListener != null) {
                    //                            mOnTagChangeListener.onDelete(TagGroup.this, lastNormalTagView.Text);
                    //                        }
                    //                    } else {
                    //                        TagView checkedTagView = getCheckedTag();
                    //                        if (checkedTagView != null) {
                    //                            checkedTagView.setChecked(false);
                    //                        }
                    //                        lastNormalTagView.setChecked(true);
                    //                    }
                    //                    return true;
                    //                }
                    //            }
                    //        }
                    //        return false;
                    //    }
                    //});

                    // Handle the INPUT tag content changed.
                    //addTextChangedListener(new TextWatcher() {
                    //    @Override
                    //    public void beforeTextChanged(CharSequence s, int start, int count, int after) {
                    //        // When the INPUT state tag changed, uncheck the checked tag if exists.
                    //        TagView checkedTagView = getCheckedTag();
                    //        if (checkedTagView != null) {
                    //            checkedTagView.setChecked(false);
                    //        }
                    //    }

                    //    @Override
                    //    public void onTextChanged(CharSequence s, int start, int before, int count) {
                    //    }

                    //    @Override
                    //    public void afterTextChanged(Editable s) {
                    //    }
                    //});
                }

                invalidatePaint();
            }

            /**
             * Set whether this tag view is in the checked state.
             *
             * @param checked true is checked, false otherwise
             */
            public void setChecked(bool check)
            {
                isChecked = check;
                // Make the checked mark drawing region.
                SetPadding(tagGroup.horizontalPadding,
                        tagGroup.verticalPadding,
                        isChecked ? (int)(tagGroup.horizontalPadding + Height / 2.5f + CHECKED_MARKER_OFFSET)
                                : tagGroup.horizontalPadding,
                        tagGroup.verticalPadding);
                invalidatePaint();
            }

            /**
             * Call this method to end this tag's INPUT state.
             */
            public void endInput()
            {
                // Make the view not focusable.
                Focusable = (false);
                FocusableInTouchMode = (false);
                // Set the hint empty, make the TextView measure correctly.
                Hint = (null);
                // Take away the cursor.
                MovementMethod = (null);

                mState = STATE_NORMAL;
                invalidatePaint();
                RequestLayout();
            }

            protected override bool DefaultEditable => true;

            /**
             * Indicates whether the input content is available.
             *
             * @return True if the input content is available, false otherwise.
             */
            public bool isInputAvailable()
            {
                return Text != null && Text.Length > 0;
            }

            private void invalidatePaint()
            {
                if (tagGroup.isAppendMode)
                {
                    if (mState == STATE_INPUT)
                    {
                        mBorderPaint.Color = new Color(tagGroup.dashBorderColor);
                        mBorderPaint.SetPathEffect(mPathEffect);
                        mBackgroundPaint.Color = new Color(tagGroup.backgroundColor);
                        SetHintTextColor(ColorStateList.ValueOf(new Color(tagGroup.inputHintColor)));
                        SetTextColor(ColorStateList.ValueOf(new Color(tagGroup.inputTextColor)));
                    }
                    else
                    {
                        mBorderPaint.SetPathEffect(null);
                        if (isChecked)
                        {
                            mBorderPaint.Color = new Color(tagGroup.checkedBorderColor);
                            mBackgroundPaint.Color = new Color(tagGroup.checkedBackgroundColor);
                            SetTextColor(ColorStateList.ValueOf(new Color(tagGroup.checkedTextColor)));
                        }
                        else
                        {
                            mBorderPaint.Color = new Color(tagGroup.borderColor);
                            mBackgroundPaint.Color = new Color(tagGroup.backgroundColor);
                            SetTextColor(ColorStateList.ValueOf(new Color(tagGroup.textColor)));
                        }
                    }
                }
                else
                {
                    if (color != null)
                    {
                        mBorderPaint.Color = new Color(color.borderColor);
                        mBackgroundPaint.Color = new Color(color.backgroundColor);
                        SetTextColor(ColorStateList.ValueOf(new Color(tagGroup.textColor)));
                    }
                    else
                    {
                        mBorderPaint.Color = new Color(tagGroup.borderColor);
                        mBackgroundPaint.Color = new Color(tagGroup.backgroundColor);
                        SetTextColor(ColorStateList.ValueOf(new Color(tagGroup.textColor)));
                    }
                }

                if (isPressed)
                {
                    mBackgroundPaint.Color = new Color(tagGroup.pressedBackgroundColor);
                }
            }

            protected override void OnDraw(Canvas canvas)
            {
                //canvas.drawArc(mLeftCornerRectF, -180, 90, true, mBackgroundPaint);
                //canvas.drawArc(mLeftCornerRectF, -270, 90, true, mBackgroundPaint);
                //canvas.drawArc(mRightCornerRectF, -90, 90, true, mBackgroundPaint);
                //canvas.drawArc(mRightCornerRectF, 0, 90, true, mBackgroundPaint);
                canvas.DrawRect(mHorizontalBlankFillRectF, mBackgroundPaint);
                canvas.DrawRect(mVerticalBlankFillRectF, mBackgroundPaint);

                if (isChecked)
                {
                    canvas.Save();
                    canvas.Rotate(45, mCheckedMarkerBound.CenterX(), mCheckedMarkerBound.CenterY());
                    canvas.DrawLine(mCheckedMarkerBound.Left, mCheckedMarkerBound.CenterY(),
                            mCheckedMarkerBound.Right, mCheckedMarkerBound.CenterY(), mCheckedMarkerPaint);
                    canvas.DrawLine(mCheckedMarkerBound.CenterX(), mCheckedMarkerBound.Top,
                            mCheckedMarkerBound.CenterX(), mCheckedMarkerBound.Bottom, mCheckedMarkerPaint);
                    canvas.Restore();
                }
                canvas.DrawPath(mBorderPath, mBorderPaint);
                base.OnDraw(canvas);
            }

            protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
            {
                base.OnSizeChanged(w, h, oldw, oldh);
                int left = (int)tagGroup.borderStrokeWidth;
                int top = (int)tagGroup.borderStrokeWidth;
                int right = (int)(left + w - tagGroup.borderStrokeWidth * 2);
                int bottom = (int)(top + h - tagGroup.borderStrokeWidth * 2);

                int d = 0;//bottom - top;

                mLeftCornerRectF.Set(left, top, left + d, top + d);
                mRightCornerRectF.Set(right - d, top, right, top + d);

                mBorderPath.Reset();
                //mBorderPath.addArc(mLeftCornerRectF, -180, 90);
                //mBorderPath.addArc(mLeftCornerRectF, -270, 90);
                //mBorderPath.addArc(mRightCornerRectF, -90, 90);
                //mBorderPath.addArc(mRightCornerRectF, 0, 90);

                int l = (int)(d / 2.0f);
                mBorderPath.MoveTo(left + l, top);
                mBorderPath.LineTo(right - l, top);

                mBorderPath.MoveTo(left + l, bottom);
                mBorderPath.LineTo(right - l, bottom);

                mBorderPath.MoveTo(left, top + l);
                mBorderPath.LineTo(left, bottom - l);

                mBorderPath.MoveTo(right, top + l);
                mBorderPath.LineTo(right, bottom - l);

                mHorizontalBlankFillRectF.Set(left, top + l, right, bottom - l);
                mVerticalBlankFillRectF.Set(left + l, top, right - l, bottom);

                int m = (int)(h / 2.5f);
                h = bottom - top;
                mCheckedMarkerBound.Set(right - m - tagGroup.horizontalPadding + CHECKED_MARKER_OFFSET,
                        top + h / 2 - m / 2,
                        right - tagGroup.horizontalPadding + CHECKED_MARKER_OFFSET,
                        bottom - h / 2 + m / 2);

                // Ensure the checked mark drawing region is correct across screen orientation changes.
                if (isChecked)
                {
                    SetPadding(tagGroup.horizontalPadding,
                            tagGroup.verticalPadding,
                            (int)(tagGroup.horizontalPadding + h / 2.5f + CHECKED_MARKER_OFFSET),
                            tagGroup.verticalPadding);
                }
            }

            public override bool OnTouchEvent(MotionEvent e)
            {
                if (mState == STATE_INPUT)
                {
                    // The INPUT tag doesn't change background color on the touch event.
                    return base.OnTouchEvent(e);
                }

                switch (e.Action)
                {
                    case MotionEventActions.Down:
                        {
                            GetDrawingRect(mOutRect);
                            isPressed = true;
                            invalidatePaint();
                            Invalidate();
                            break;
                        }
                    case MotionEventActions.Move:
                        {
                            if (!mOutRect.Contains((int)e.GetX(), (int)e.GetY()))
                            {
                                isPressed = false;
                                invalidatePaint();
                                Invalidate();
                            }
                            break;
                        }
                    case MotionEventActions.Up:
                        {
                            isPressed = false;
                            invalidatePaint();
                            Invalidate();
                            break;
                        }
                }
                return base.OnTouchEvent(e);
            }

            public override IInputConnection OnCreateInputConnection(EditorInfo outAttrs)
            {
                return new ZanyInputConnection(base.OnCreateInputConnection(outAttrs), true);
            }

            /**
             * Solve edit text delete(backspace) key detect, see<a href="http://stackoverflow.com/a/14561345/3790554">
             * Android: Backspace in WebView/BaseInputConnection</a>
             */
            class ZanyInputConnection : InputConnectionWrapper
            {
                public ZanyInputConnection(IInputConnection target, bool mutable) : base(target, mutable)
                {

                }

                public override bool DeleteSurroundingText(int beforeLength, int afterLength)
                {
                    // magic: in latest Android, deleteSurroundingText(1, 0) will be called for backspace
                    if (beforeLength == 1 && afterLength == 0)
                    {
                        // backspace
                        return SendKeyEvent(new KeyEvent(KeyEventActions.Down, Keycode.Del))
                                && SendKeyEvent(new KeyEvent(KeyEventActions.Up, Keycode.Del));
                    }
                    return base.DeleteSurroundingText(beforeLength, afterLength);
                }
            }
        }
    }
}