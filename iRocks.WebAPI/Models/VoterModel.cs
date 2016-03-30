using iRocks.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRocks.WebAPI.Models
{
    public class VoterModel
    {
        public VoterModel(Vote vote, AppUserModel voter)
        {
            this.Vote = vote;
            this.Voter = voter;
        }
        public Vote Vote { get; set; }
        public AppUserModel Voter { get; set; }
    }
}