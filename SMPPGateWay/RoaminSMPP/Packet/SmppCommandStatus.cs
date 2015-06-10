using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoaminSMPP.Packet
{
    public enum SmppCommandStatus
    {
        /// <summary>
        ///  No Error
        /// </summary>
        ESME_ROK = 0x00000000,
        /// <summary>
        /// Message Length is invalid
        /// </summary>
        ESME_RINVMSGLEN = 0x00000001,
        /// <summary>
        /// Command Length is invalid
        /// </summary>
        ESME_RINVCMDLEN = 0x00000002,
        /// <summary>
        /// Invalid Command ID
        /// </summary>
        ESME_RINVCMDID = 0x00000003,
        /// <summary>
        /// Incorrect BIND Status for given command
        /// </summary>
        ESME_RINVBNDSTS = 0x00000004,
        /// <summary>
        /// ESME Already in Bound State
        /// </summary>
        ESME_RALYBND = 0x00000005,
        /// <summary>
        ///  Invalid Priority Flag
        /// </summary>
        ESME_RINVPRTFLG = 0x00000006,
        /// <summary>
        /// Invalid Registered Delivery Flag
        /// </summary>
        ESME_RINVREGDLVFLG = 0x00000007, 
        /// <summary>
        /// System Error
        /// </summary>
        ESME_RSYSERR = 0x00000008, 
        /// <summary>
        /// Invalid Source Address
        /// </summary>
        ESME_RINVSRCADR = 0x0000000A,
        /// <summary>
        /// Invalid Dest Addr
        /// </summary>
        ESME_RINVDSTADR = 0x0000000B,
        /// <summary>
        /// Message ID is invalid
        /// </summary>
        ESME_RINVMSGID = 0x0000000C,
        /// <summary>
        /// Bind Failed
        /// </summary>
        ESME_RBINDFAIL = 0x0000000D,
        /// <summary>
        /// Invalid Password
        /// </summary>
        ESME_RINVPASWD = 0x0000000E,
        /// <summary>
        /// Invalid System ID
        /// </summary>
        ESME_RINVSYSID = 0x0000000F,
        /// <summary>
        /// Cancel SM Failed
        /// </summary>
        ESME_RCANCELFAIL = 0x00000011,
        /// <summary>
        /// Replace SM Failed
        /// </summary>
        ESME_RREPLACEFAIL = 0x00000013,
        /// <summary>
        /// Message Queue Full
        /// </summary>
        ESME_RMSGQFUL = 0x00000014,
        /// <summary>
        /// Invalid Service Type
        /// </summary>
        ESME_RINVSERTYP = 0x00000015, 
        /// <summary>
        /// Invalid number of destinations
        /// </summary>
        ESME_RINVNUMDESTS = 0x00000033,
        /// <summary>
        /// Invalid Distribution List name
        /// </summary>
        ESME_RINVDLNAME = 0x00000034,
        /// <summary>
        /// Destination flag is invalid (submit_multi)
        /// </summary>
        ESME_RINVDESTFLAG = 0x00000040,
        /// <summary>
        /// Invalid ‘submit with replace’ request (i.e. submit_sm with replace_if_present_flag set)
        /// </summary>
        ESME_RINVSUBREP = 0x00000042,
        /// <summary>
        /// Invalid esm_class field data
        /// </summary>
        ESME_RINVESMCLASS = 0x00000043,
        /// <summary>
        /// Cannot Submit to Distribution List
        /// </summary>
        ESME_RCNTSUBDL = 0x00000044,
        /// <summary>
        /// submit_sm or submit_multi failed
        /// </summary>
        ESME_RSUBMITFAIL = 0x00000045,
        /// <summary>
        /// Invalid Source address TON
        /// </summary>
        ESME_RINVSRCTON = 0x00000048,
        /// <summary>
        /// Invalid Source address NPI
        /// </summary>
        ESME_RINVSRCNPI = 0x00000049,
        /// <summary>
        /// Invalid Destination address TON
        /// </summary>
        ESME_RINVDSTTON = 0x00000050,
        /// <summary>
        /// Invalid Destination address NPI
        /// </summary>
        ESME_RINVDSTNPI = 0x00000051, 
        /// <summary>
        /// Invalid system_type field
        /// </summary>
        ESME_RINVSYSTYP = 0x00000053,
        /// <summary>
        /// Invalid replace_if_present flag
        /// </summary>
        ESME_RINVREPFLAG = 0x00000054,
        /// <summary>
        /// Invalid number of messages
        /// </summary>
        ESME_RINVNUMMSGS = 0x00000055,
        /// <summary>
        /// Throttling error (ESME has exceeded allowed message limits)
        /// </summary>
        ESME_RTHROTTLED = 0x00000058,
        /// <summary>
        /// Invalid Scheduled Delivery Time
        /// </summary>
        ESME_RINVSCHED = 0x00000061,
        /// <summary>
        /// Invalid message validity period (Expiry time)
        /// </summary>
        ESME_RINVEXPIRY = 0x00000062,
        /// <summary>
        /// Predefined Message Invalid or Not Found
        /// </summary>
        ESME_RINVDFTMSGID = 0x00000063,
        /// <summary>
        /// ESME Receiver Temporary App Error Code
        /// </summary>
        ESME_RX_T_APPN = 0x00000064,
        /// <summary>
        /// ESME Receiver Permanent App Error Code
        /// </summary>
        ESME_RX_P_APPN = 0x00000065,
        /// <summary>
        /// ESME Receiver Reject Message Error Code
        /// </summary>
        ESME_RX_R_APPN = 0x00000066,
        /// <summary>
        /// query_sm request failed
        /// </summary>
        ESME_RQUERYFAIL = 0x00000067,
        /// <summary>
        /// Error in the optional part of the PDU Body.
        /// </summary>
        ESME_RINVOPTPARSTREAM = 0x000000C0,
        /// <summary>
        /// Optional Parameter not allowed
        /// </summary>
        ESME_ROPTPARNOTALLWD = 0x000000C1, 
        /// <summary>
        /// Invalid Parameter Length.
        /// </summary>
        ESME_RINVPARLEN = 0x000000C2, 
        /// <summary>
        /// Expected Optional Parameter missing
        /// </summary>
        ESME_RMISSINGOPTPARAM = 0x000000C3,
        /// <summary>
        /// Invalid Optional Parameter Value
        /// </summary>
        ESME_RINVOPTPARAMVAL = 0x000000C4,
        /// <summary>
        /// Delivery Failure (used for data_sm_resp)
        /// </summary>
        ESME_RDELIVERYFAILURE = 0x000000FE, 
        /// <summary>
        /// Unknown Error
        /// </summary>
        ESME_RUNKNOWNERR = 0x000000FF
    }
}
