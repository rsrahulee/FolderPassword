using System.Windows;
using System.IO;
using System.Xml;
using System;
using System.Security.AccessControl;

namespace PasswordProtection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private String hexID = ".{2559a1f2-21d7-11d4-bdaf-00c04f60b9f0}";//class ID from the Windows registry, it saves the file with ".bat" extension
        private String directory = "C:\\RR";
        private String dirPin = "\\pin.xml";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLock(object sender, RoutedEventArgs e)
        {
            if (!(boxPassword.Password.Equals("")))
            {
                DirectoryInfo d = new DirectoryInfo(directory);
                String selectedpath = d.Parent.FullName + d.Name;

                XmlDocument xmldoc = new XmlDocument();
                XmlElement xmlelem;
                XmlNode xmlnode;
                XmlText xmltext;
                xmlnode = xmldoc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
                xmldoc.AppendChild(xmlnode);
                xmlelem = xmldoc.CreateElement("", "ROOT", "");
                xmltext = xmldoc.CreateTextNode(boxPassword.Password);
                xmlelem.AppendChild(xmltext);
                xmldoc.AppendChild(xmlelem);
                xmldoc.Save(selectedpath + dirPin);

                if (!d.Root.Equals(d.Parent.FullName))
                    d.MoveTo(d.Parent.FullName + "\\" + d.Name + hexID);
            }
        }

        private void btnUnlock(object sender, RoutedEventArgs e)
        {
            DirectoryInfo d = new DirectoryInfo(directory + hexID);
            String selectedpath = d.Parent.FullName + d.Name;

            bool s = checkpassword(selectedpath);
            if (s)
            {
                modifyFolder();

                File.Delete(selectedpath + dirPin);
                d.MoveTo(selectedpath.Substring(0, selectedpath.LastIndexOf(".")));
            }
            else
            {
                MessageBox.Show("Incorrect Password");
            }
        }

        private bool checkpassword(String selectedpath)
        {
            if (!(boxPassword.Password.ToString().Equals("")))
            {
                XmlTextReader read = new XmlTextReader(selectedpath + dirPin);

                if (read.ReadState == ReadState.Error)
                    return true;
                else
                {
                    try
                    {
                        while (read.Read())
                            if (read.NodeType == XmlNodeType.Text)
                            {
                                if (boxPassword.Password.Equals(read.Value))
                                {
                                    read.Close();
                                    return true;
                                }
                                else
                                {
                                    read.Close();
                                    return false;
                                }
                            }
                    }
                    catch
                    {
                        return true;
                    }
                }
                read.Close();
            }
            return false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(directory + hexID))
            {
                DirectoryInfo di = Directory.CreateDirectory(directory + hexID + "\\Rahul");
            }

            doNotModifyFolder();
        }

        private void doNotModifyFolder()//do not allow to modify
        {
            try
            {
                string adminUserName = Environment.UserName;
                DirectorySecurity ds = Directory.GetAccessControl(directory + hexID);
                FileSystemAccessRule fsa = new FileSystemAccessRule(adminUserName, FileSystemRights.Modify, AccessControlType.Deny);

                ds.AddAccessRule(fsa);//add
                Directory.SetAccessControl(directory + hexID, ds);
            }
            catch (Exception)
            {

            }
        }

        private void modifyFolder()//allow to modify
        {
            try
            {
                string adminUserName = Environment.UserName;
                DirectorySecurity ds = Directory.GetAccessControl(directory + hexID);
                FileSystemAccessRule fsa = new FileSystemAccessRule(adminUserName, FileSystemRights.Modify, AccessControlType.Deny);

                ds.RemoveAccessRule(fsa);//remove
                Directory.SetAccessControl(directory + hexID, ds);
            }
            catch (Exception)
            {

            }
        }
    }
}
