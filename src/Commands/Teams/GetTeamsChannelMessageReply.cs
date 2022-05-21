﻿using PnP.PowerShell.Commands.Attributes;
using PnP.PowerShell.Commands.Base;
using PnP.PowerShell.Commands.Base.PipeBinds;
using PnP.PowerShell.Commands.Utilities;
using System.Management.Automation;

namespace PnP.PowerShell.Commands.Teams
{
    [Cmdlet(VerbsCommon.Get, "PnPTeamsChannelMessageReply")]
    [RequiredMinimalApiPermissions("ChannelMessage.Read.All")]
    public class GetTeamsChannelMessageReply : PnPGraphCmdlet
    {
        [Parameter(Mandatory = true)]
        public TeamsTeamPipeBind Team;

        [Parameter(Mandatory = true)]
        public TeamsChannelPipeBind Channel;

        [Parameter(Mandatory = true)]
        public TeamsChannelMessagePipeBind Message;

        [Parameter(Mandatory = false)]
        public TeamsChannelMessageReplyPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            var groupId = Team.GetGroupId(HttpClient, AccessToken);
            if (groupId == null)
            {
                throw new PSArgumentException("Group not found");
            }

            var channelId = Channel.GetId(HttpClient, AccessToken, groupId);
            if (channelId == null)
            {
                throw new PSArgumentException("Channel not found");
            }

            var messageId = Message.GetId();
            if (messageId == null)
            {
                throw new PSArgumentException("Message not found");
            }

            try
            {
                if (ParameterSpecified(nameof(Identity)))
                {
                    var reply = TeamsUtility.GetMessageReplyAsync(HttpClient, AccessToken, groupId, channelId, messageId, Identity.GetId()).GetAwaiter().GetResult();
                    WriteObject(reply);
                }
                else
                {
                    var replies = TeamsUtility.GetMessageRepliesAsync(HttpClient, AccessToken, groupId, channelId, messageId).GetAwaiter().GetResult();
                    WriteObject(replies, true);
                }
            }
            catch
            {
                // Exception thrown by Graph is quite unclear.
                var message = ParameterSpecified(nameof(Identity)) ? "Failed to retrieve reply." : "Failed to retrieve replies.";
                throw new PSArgumentException(message);
            }
        }
    }
}
