using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Xamarin.Forms;

namespace PathwayGames.Controls
{
    public class UniformImageGrid : Grid
    {
        public static readonly BindableProperty ColumnCountProperty =
            BindableProperty.Create(nameof(ColumnCount), typeof(int), typeof(UniformImageGrid), 1, propertyChanged: (b, o, n) => {
                UniformImageGrid self = b as UniformImageGrid;

                if (n != null)
                {
                    self.GenerateColumns();
                }
            });

        public int ColumnCount
        {
            get { return (int)GetValue(ColumnCountProperty); }
            set { SetValue(ColumnCountProperty, value); }
        }

        private int RowCount { get; set; }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private IList<object> _internalItems;
        internal IList<object> InternalItems {
            get { return _internalItems; }
            set {
                _internalItems = value;
                Render();
            }
        }

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(UniformImageGrid), null,
                propertyChanged: (b, o, n) => {
                    UniformImageGrid self = b as UniformImageGrid;
                    //ObservableCollection Tracking 
                    if (o != null && o is INotifyCollectionChanged)
                        (o as INotifyCollectionChanged).CollectionChanged -= self.HandleItemsSourceCollectionChanged;

                    if (n != null)
                    {
                        if (n is INotifyCollectionChanged)
                            (n as INotifyCollectionChanged).CollectionChanged += self.HandleItemsSourceCollectionChanged;

                        self.InternalItems = new List<object>(((IEnumerable)n).Cast<object>());
                    } else {
                        self.InternalItems = null;
                    }
                });

        void HandleItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            InternalItems = new List<object>(((IEnumerable)sender).Cast<object>());
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
           // SetColumnsBindingContext();
        }

        public UniformImageGrid()
        {
            //InitializeComponent();
        }

        private void Render()
        {
            Children?.Clear();
            if (InternalItems != null && InternalItems.Count() > 0)
            {
                GenerateRows();
                for (var index = 0; index < InternalItems.Count; index++)
                {
                    int col = (int)(index % ColumnCount);
                    var row = (int)Math.Floor(index / (float)ColumnCount);

                    var image = CreateImage(InternalItems[index]); 

                    Children?.Add(image, col, row);
                }
            }
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            SizeRequest sizeRequest = base.OnMeasure(double.PositiveInfinity, double.PositiveInfinity);
            if (sizeRequest.Request.IsZero)
                return sizeRequest;
            //System.Diagnostics.Debug.WriteLine($"WC - {widthConstraint} : HC - {heightConstraint}");
           // System.Diagnostics.Debug.WriteLine($"WB - {sizeRequest.Request.Width} : HB - {sizeRequest.Request.Height}");
            //var innerAspectRatio = sizeRequest.Request.Width / sizeRequest.Request.Height;
            var innerAspectRatio = ColumnCount / RowCount;
            if (double.IsInfinity(heightConstraint))
            {
                if (double.IsInfinity(widthConstraint))
                {
                    // both constraints are infinity
                    // use the view's size request dimensions
                    widthConstraint = sizeRequest.Request.Width;
                    heightConstraint = sizeRequest.Request.Height;
                }
                else
                {
                    // Height constraint is infinity
                    heightConstraint = widthConstraint * sizeRequest.Request.Height / sizeRequest.Request.Width;
                }
            }
            else if (double.IsInfinity(widthConstraint))
            {
                // Width constraint is infity
                widthConstraint = heightConstraint * sizeRequest.Request.Width / sizeRequest.Request.Height;
            }
            else
            {
                // both of the destination width and height constraints are non-infinity
                var outerAspectRatio = widthConstraint / heightConstraint;

                var resizeFactor = (innerAspectRatio >= outerAspectRatio) ?
                    (widthConstraint / sizeRequest.Request.Width) :
                    (heightConstraint / sizeRequest.Request.Height);

                widthConstraint = sizeRequest.Request.Width * resizeFactor;
                heightConstraint = sizeRequest.Request.Height * resizeFactor;
            }

            return new SizeRequest(new Size(widthConstraint, heightConstraint));
        }

        private void GenerateRows()
        {
            RowCount = (int)Math.Ceiling((double)InternalItems.Count / ColumnCount);
            if (RowCount != RowDefinitions.Count)
            {
                // Create rows
                RowDefinitions?.Clear();

                for (int i = 0; i < RowCount; i++)
                {
                    RowDefinitions?.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                }
            }
        }

        private void GenerateColumns()
        {
            // Create columns
            ColumnDefinitions?.Clear();
            for (int i = 0; i < ColumnCount; i++)
            {
                ColumnDefinitions?.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }
        }

        private View CreateImage(object item)
        {
            var fileName = item as string;
            var image = new FFImageLoading.Forms.CachedImage()
            {
                Source = ImageSource.FromFile(fileName),
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Aspect = Aspect.AspectFill,
            };
            return image;
        }
    }
}
