using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using QSharp;
using Serilog;

namespace QSharpDemoApp
{
    public partial class Form1 : Form
    {
        QBrowser browser;
        QWorkspace connectedWorkspace;
        public Form1()
        {
            InitializeComponent();
            browser = new QBrowser();
            browser.ServerFound += Browser_ServerFound;
        }

        private void Browser_ServerFound(object source, QServerFoundArgs args)
        {
            Log.Debug(args.server.name);
            args.server.ServerUpdated += Server_ServerUpdated;
            args.server.refreshWorkspaces();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().MinimumLevel.Debug().CreateLogger();
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            browser.Close();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if(serverListView.SelectedItems.Count == 1)
            {
                
                QServer server = browser.serverForAddress(serverListView.SelectedItems[0].SubItems[3].Text);
                string workspaceID = serverListView.SelectedItems[0].SubItems[2].Text;
                if (server != null)
                {
                    connectedWorkspace = server.workspaceWithID(workspaceID);
                    if(connectedWorkspace != null)
                    {
                        connectedWorkspace.WorkspaceUpdated += Workspace_WorkspaceUpdated;
                        connectedWorkspace.connectWithPasscode("");
                    }
                }
            }
        }

        private void Server_ServerUpdated(object source, QServerUpdatedArgs args)
        {
            QServer server = args.server;

            if (server.workspaces.Count > 0)
            {
                Log.Debug($"[demo] server has more than one workspace so adding as a group");

                if (serverListView.InvokeRequired)
                {
                    serverListView.Invoke((MethodInvoker)delegate () {
                        ListViewGroup serverGroup = new ListViewGroup(server.name.ToUpper());
                        serverListView.Groups.Add(serverGroup);
                        foreach(var workspace in server.workspaces)
                        {
                            ListViewItem workspaceItem = new ListViewItem(new[] { workspace.nameWithoutPathExtension, workspace.version, workspace.uniqueID, server.host }, serverGroup) ;
                            serverListView.Items.Add(workspaceItem);
                        }
                    });
                }
            }
        }

        private void reloadListView()
        {
            if (workspaceListView.InvokeRequired)
            {
                workspaceListView.Invoke((MethodInvoker)delegate () {

                    workspaceListView.Items.Clear();

                    foreach(var cueList in connectedWorkspace.cueLists)
                    {
                        addCueToListView(cueList);
                    }

                    workspaceListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    workspaceListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

                });
            }
        }

        public void addCueToListView(QCue cue)
        {

            if (cue.IsGroup)
            {
                ListViewGroup cueGroup = new ListViewGroup(cue.displayName);
                workspaceListView.Groups.Add(cueGroup);
                foreach(var aCue in cue.cues)
                {
                    ListViewItem cueItem = new ListViewItem(new[] { aCue.number, aCue.listName, aCue.uid }, cueGroup);
                    cueItem.BackColor = ColorTranslator.FromHtml($"#{aCue.color.lightHex}");
                    workspaceListView.Items.Add(cueItem);
                }
            }
        }

        private void Workspace_WorkspaceUpdated(object source, QWorkspaceUpdatedArgs args)
        {
            Log.Debug("[demo] workspace has been updated");
            if(connectedWorkspace.firstCueList != null)
            {
                reloadListView();
            }
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            if (connectedWorkspace != null)
                connectedWorkspace.go();
        }
    }
}
