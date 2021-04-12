
namespace ZDC2911Demo.SysEnum {
    public enum UserPrivilege {
        /// <summary>
        /// 普通用户
        /// </summary>
        ROLE_GENERAL_USER = 1,

        /// <summary>
        /// 登记注册员
        /// </summary>
        ROLE_ENROLL_USER = 2,

        /// <summary>
        /// 记录查询员
        /// </summary>
        ROLE_VIEW_USER = 4,

        /// <summary>
        /// 超级管理员
        /// </summary>
        ROLE_SUPER_USER = 8,

        /// <summary>
        /// 客户
        /// </summary>
        ROLE_CUSTOMER = 16
    }
}
