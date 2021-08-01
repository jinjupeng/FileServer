using System;

namespace FileServer.Aliyun
{
    public class AliyunOptions
    {
        public string AccessKeyId { get; set; }

        public string AccessKeySecret { get; set; }

        public string Endpoint { get; set; }

        public bool UseSecurityTokenService { get; set; }

        public string RegionId { get; set; }

        /// <summary>
        /// acs:ram::$accountID:role/$roleName
        /// </summary>
        public string RoleArn { get; set; }

        /// <summary>
        /// The name used to identify the temporary access credentials, it is recommended to use different application users to distinguish.
        /// </summary>
        public string RoleSessionName { get; set; }

        /// <summary>
        /// Set the validity period of the temporary access credential, the unit is s, the minimum is 900, and the maximum is 3600.
        /// </summary>
        public int DurationSeconds { get; set; }

        /// <summary>
        /// If policy is empty, the user will get all permissions under this role
        /// </summary>
        public string Policy { get; set; }
    }
}
