using LibGit2Sharp;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Updater
{
    public partial class Form1 : Form
    {
        string githubUrl = @"https://github.com/reAudioPlayer-Apollo/release";
        string updaterUrl = @"https://github.com/reAudioPlayer-Apollo/installer";
        string repoLocation = "reAudioPlayer Apollo/Player";
        string updaterLocation = "reAudioPlayer Apollo/Updater";

        string pullPath = "../Player";

        public Form1()
        {
            InitializeComponent();

            var loc = AppDomain.CurrentDomain.BaseDirectory;
            var parent = new DirectoryInfo(loc).Parent.Name;

            if (parent == "reAudioPlayer Apollo")
            {
                //MessageBox.Show("updating!");
                update();
            }
        }

        private void btnInstall_Click(object sender, EventArgs e) // clone
        {
            Task.Factory.StartNew(() => install());
        }

        void install()
        {
            lblStatus.Invoke(new Action(() =>
            {
                lblStatus.Text = "Downloading player (1 / 2)...";
            }));

            Repository.Clone(githubUrl, repoLocation);

            lblStatus.Invoke(new Action(() =>
            {
                lblStatus.Text = "Downloading updater (2 / 2)...";
            }));

            Repository.Clone(updaterUrl, updaterLocation);

            lblStatus.Invoke(new Action(() =>
            {
                lblStatus.Text = "Finished download & installation!";
            }));

            btnInstall.Invoke(new Action(() =>
            {
                btnInstall.Text = "Finish";
                btnInstall.Click -= btnInstall_Click;
                btnInstall.Click += btnExit_Click;
            }));
        }

        private void update() // pull
        {
            using (var repo = new Repository(pullPath))
            {
                var sig = new Signature("test", "test@test.com", new DateTimeOffset(DateTime.Now));
                var opt = new PullOptions();
                opt.MergeOptions = new MergeOptions();
                opt.MergeOptions.FileConflictStrategy = CheckoutFileConflictStrategy.Theirs;
                opt.MergeOptions.MergeFileFavor = MergeFileFavor.Theirs;

                repo.Stashes.Add(sig, "Stash on master");

                Commands.Pull(repo, sig, opt);
                /*
                var remote = repo.Network.Remotes["origin"];
                var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                Commands.Fetch(repo, remote.Name, refSpecs, null, logMessage);*/

                //repo.Stashes.Apply(0);
            }
            //MessageBox.Show("successfully updated!");
            Environment.Exit(0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(-1);
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
