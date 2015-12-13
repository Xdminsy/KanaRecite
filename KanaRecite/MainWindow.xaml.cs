using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KanaRecite
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        static Dictionary<string, string> HiraganaDict = new Dictionary<string, string>(48);
        static string hText = "あ,い,う,え,お,か,き,く,け,こ,が,ぎ,ぐ,げ,ご,さ,し,す,せ,そ,ざ,じ,ず,ぜ,ぞ,た,ち,つ,て,と,だ,ぢ,づ,で,ど,な,に,ぬ,ね,の,は,ひ,ふ,へ,ほ,ば,び,ぶ,べ,ぼ,ぱ,ぴ,ぷ,ぺ,ぽ,ま,み,む,め,も,や,ゆ,よ,ら,り,る,れ,ろ,わ,を,ん,きゃ,きゅ,きょ,ぎゃ,ぎゅ,ぎょ,しゃ,しゅ,しょ,じゃ,じゅ,じょ,ちゃ,ちゅ,ちょ,にゃ,にゅ,にょ,ひゃ,ひゅ,ひょ,びゃ,びゅ,びょ,ぴゃ,ぴゅ,ぴょ,みゃ,みゅ,みょ,りゃ,りゅ,りょ";
        static string sText = "a,i,u,e,o,ka,ki,ku,ke,ko,ga,gi,gu,ge,go,sa,shi,su,se,so,za,ji,zu,ze,zo,ta,chi,tsu,te,to,da,ji,zu,de,do,na,ni,nu,ne,no,ha,hi,fu,he,ho,ba,bi,bu,be,bo,pa,pi,pu,pe,po,ma,mi,mu,me,mo,ya,yu,yo,ra,ri,ru,re,ro,wa,wo,n/m,kya,kyu,kyo,gya,gyu,gyo,sha,shu,sho,ja,ju,jo,cha,chu,cho,nya,nyu,nyo,hya,hyu,hyo,bya,byu,byo,pya,pyu,pyo,mya,myu,myo,rya,ryu,ryo";
        static string[] hArray = hText.Split(',');
        static string[] sArray = sText.Split(',');
        static string[] praise = {"Awesome!", "You are right", "Good", "Great", "Fancy!~", "Yeah~", "Fantasic!!", "Yooooo"};
        bool Checked;
        bool Emptyed;
        public MainWindow()
        {
            InitializeComponent();
            
            for (int i = 0; i < 104; i++)
            {
                HiraganaDict.Add(hArray[i], sArray[i]);
            }
        }

        Random r = new Random();
        private void nextKana(){
            var index = r.Next(104);
            string next;
            while((next = hArray[index]) == Gana.Content.ToString());
            Gana.Content = next;
            commentOut();
            SoundTextBox.Text = "";
            SoundTextBox.Focus();
            Checked = false;
        }
        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            
            var leftAnimation = new DoubleAnimation(Left, Left + Width / 2, TimeSpan.FromMilliseconds(160));
            var widthAnimation = new DoubleAnimation(Width, 0, TimeSpan.FromMilliseconds(160));
            widthAnimation.Completed += (s, ev) => Application.Current.Shutdown();
            BeginAnimation(LeftProperty, leftAnimation);
            BeginAnimation(WidthProperty, widthAnimation);
        }

        private void WinMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void WinLoaded(object sender, RoutedEventArgs e)
        {
            var opacityAnimation = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
            var heightAnimation = new DoubleAnimation(0, Height, TimeSpan.FromMilliseconds(300));
            BeginAnimation(OpacityProperty, opacityAnimation);
            BeginAnimation(HeightProperty, heightAnimation);
        }

        private void NextButtonClick(object sender, RoutedEventArgs e)
        {
            nextKana();
        }
        private void check()
        {
            var sound = HiraganaDict[Gana.Content.ToString().Trim()].Trim();
            string content;
            if (string.IsNullOrEmpty(SoundTextBox.Text.Trim()))
            {
                Comment.Content = "You typed nothing.";
                if (Emptyed) {
                    commentFlash();
                } else { 
                    Emptyed = true;
                    commentIn();
                }
            }
            else
            {
                Emptyed = false;
                if (SoundTextBox.Text.Trim() == sound)
                {
                    Comment.Content = praise[r.Next(praise.Length)];
                    commentIn((s, e) => { commentFlash((s2, e2) => { commentOut((s3, e3) => { nextKana(); }); }); });
                }
                else
                {
                    Comment.content = "Sorry, it's " + sound;
                    commentIn();
                    Checked = true;
                }
            }
            SoundTextBox.Focus();
        }
        private void CheckButtonClick(object sender, RoutedEventArgs e)
        {
            if (Checked) {
                commentFlash();
            } else check();
        }

        private void SoundTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || ((e.Key == Key.J || e.Key == Key.M) && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))))
            {
                if (Checked)
                {
                    nextKana();
                }
                else
                {
                    check();
                }
            }
        }

        private void GanaLoaded(object sender, RoutedEventArgs e)
        {
            nextKana();
        }

        private void commentIn(EventHandler e = null)
        {
            var anim = new ThicknessAnimation();
            anim.From = new Thickness(-Comment.Width, Comment.Margin.Top, Comment.Margin.Right, Comment.Margin.Bottom);
            anim.To = new Thickness(0, Comment.Margin.Top, 0, Comment.Margin.Bottom);
            anim.Duration = TimeSpan.FromMilliseconds(100);
            if(e != null)anim.Completed += e;
            Comment.BeginAnimation(Label.MarginProperty, anim);
        }

        private void commentOut(EventHandler e = null)
        {
            var anim = new ThicknessAnimation();
            anim.From = Comment.Margin;
            anim.To = new Thickness(Comment.Margin.Left + Width, Comment.Margin.Top, Comment.Margin.Right, Comment.Margin.Bottom);
            anim.Duration = TimeSpan.FromMilliseconds(100);
            if (e != null) anim.Completed += e;
            Comment.BeginAnimation(Label.MarginProperty, anim);
        }

        private void commentFlash(EventHandler e = null)
        {
            var anim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(150));
            anim.RepeatBehavior = new RepeatBehavior(2);
            if (e != null) anim.Completed += e;
            Comment.BeginAnimation(Label.OpacityProperty, anim);
        }
    }
}
