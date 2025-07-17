using System;
using System.Data;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using Microsoft.UI.Xaml.Controls;
using nexus.common;
using nexus.common.cache;
using nexus.common.control;
using nexus.common.dal;
using nexus.plugins.common;

using nexus.shared.common;
using nexus.shared.promo;



namespace nexus.plugins.promo
{


    public partial class Winner : NxGroupBase 
    {

        string MemberId = "";
        string Member   = "";

        public Winner(string MemberId, string Member)
        {
            InitializeComponent();

            this.MemberId      = MemberId;
            this.Member        = Member;    

            txMember.Prompt    = Member;
        }
    }
}
