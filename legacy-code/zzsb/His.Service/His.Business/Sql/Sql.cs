using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Business.Sql
{
    public static class Sql
    {
        #region 公共

        //获取系统时间
        public static string GetSysDate = @"select sysdate from dual";

        #endregion

        #region 自助机预约取号,查询患者信息

        //根据身份证号查询信息
        public static string SelectPationinfoByIDCar = @"
                                                        select 
                                                        qwe.card_no,
                                                        qwe.name,
                                                        qwe.birthday,
                                                        qwe.sex_code,
                                                        qwe.idenno,
                                                        qwe.mcard_no,
                                                        qwe.rela_phone, 
                                                        qwe.address
                                                        from fin_opr_register qwe 
                                                        where qwe.idenno='{0}'
                                                        order by qwe.reg_date desc";

        //根据医疗证号查询信息
        public static string SelectPationinfoByMCar = @"
                                                        select 
                                                        qwe.card_no,
                                                        qwe.name,
                                                        qwe.birthday,
                                                        qwe.sex_code,
                                                        qwe.idenno,
                                                        qwe.mcard_no,
                                                        qwe.rela_phone, 
                                                        qwe.address
                                                        from fin_opr_register qwe 
                                                        where qwe.mcard_no='{0}'
                                                        order by qwe.reg_date desc";

        #endregion

        #region 锁定号源


        //获取排班剩余挂号号源
        public static string SelectSchemaRegRemain = @"select t.id,(t.reg_lmt - t.reged) regRemain
                                                          from fin_opr_schema t
                                                         where t.id = '{0}'";
        //更新排班表
        public static string UpdateSchemaReged = @"update fin_opr_schema s  --医师出诊表
                                set s.reged = s.reged + {1}, --挂号已挂
                                    s.order_no = nvl(s.order_no,0) + 1
                                where s.id = '{0}' and  s.reg_lmt>s.reged ";

        //插入号源信息表
        public static string InsertRegLock = @"insert into fin_opr_schemalock t
                                              (LOCKID,
                                               USERID,
                                               DEVICEID,
                                               SERVICECODE,
                                               FUNCODE,
                                               REQTIME,
                                               CARDNO,
                                               DEPTCODE,
                                               SESSIONCODE,
                                               DOCTORCODE,
                                               REGSOURCEID,
                                               LOCKSTATE,
                                               OPERID,
                                               seeno,
                                               OPERDATE) values
                                              ('{0}', --号源锁定ID
                                               '{1}', --用户编号
                                               '{2}', --设备编码
                                               '{3}', --服务编码
                                               '{4}', --业务编号
                                               to_date('{5}', 'yyyy-mm-dd hh24:mi:ss'), --请求时间
                                               '{6}', --卡号
                                               '{7}', --可是编码
                                               '{8}', --出诊时段编号
                                               '{9}', --医生编码
                                               '{10}', --排班ID
                                               '{11}', --锁定状态
                                               '{12}', --操作员
                                               '{13}',--看诊序号
                                               sysdate)
                                            ";
        #endregion

        #region 解锁号源

        //检索号源状态
        public static string SelectRegLock = @"select t.lockid,t.regsourceid,t.lockstate from fin_opr_schemalock t where t.lockid = '{0}'";

        //更新号源状态
        public static string UpdateRegLockState = @"update fin_opr_schemalock c
                                                       set c.lockstate = '{2}', c.operID = '{1}', c.operDate = sysdate
                                                     where c.lockid = '{0}'";

        #endregion

        #region 挂号登记

        /// <summary>
        /// 获取门诊患者登记次数
        /// </summary>
        public static string GetOutPatientInTimes = @"select count(1)+1 from fin_opr_register a where a.card_no='{0}'";

        /// <summary>
        /// 获取午别信息
        /// </summary>
        public static string GetNoonName = @"select noon_name
                                                          FROM fin_opr_noon   --午别表
                                                         WHERE noon_code='{0}'";

        /// <summary>
        /// 获取seeNo
        /// </summary>
//        public static string GetSeeNo = @"select sum(see_no)
//                                              from (select sum(a.reged) + sum(a.tel_lmt) as see_no
//                                                      from fin_opr_schema a
//                                                     where a.see_date = to_date('{0}', 'yyyy-mm-dd hh24:mi:ss')
//                                                       and a.noon_code = '{1}'
//                                                       and (a.doct_code = '{2}' or a.dept_code = '{3}')
//                                                       and a.schema_type = '{4}')
//                                            ";
        public static string GetSeeNo = @"
                          select sum(see_no) from (
                          select sum(a.tel_reging)+sum(a.reg_lmt)+sum(a.spe_lmt) as see_no
                          from fin_opr_schema a
                          where a.see_date=to_date('{0}','yyyy-mm-dd hh24:mi:ss') 
                          and a.noon_code='{1}'
                          and (a.doct_code='{2}' or a.dept_code='{3}')
                          and a.schema_type='{4}'
                          and a.end_time< to_date('{5}','yyyy-mm-dd hh24:mi:ss')
                          union all
                          select sum(a.reged)+sum(a.tel_reging)+sum(a.spe_lmt) as see_no
                          from fin_opr_schema a
                          where a.see_date=to_date('{0}','yyyy-mm-dd hh24:mi:ss') 
                          and a.noon_code='{1}'
                          and (a.doct_code='{2}' or a.dept_code='{3}')
                          and a.schema_type='{4}'
                          and a.end_time= to_date('{5}','yyyy-mm-dd hh24:mi:ss')
                          )";

        public static string bookSeeNO = @"select sum(see_no) from (
                        select sum(a.reg_lmt)+sum(a.tel_reging)+sum(a.spe_lmt) as see_no
                        from fin_opr_schema a
                        where a.see_date=to_date('{0}','yyyy-mm-dd hh24:mi:ss') 
                        and a.noon_code='{1}'
                        and (a.doct_code='{2}' /*or a.dept_code='{3}'*/)
                        and a.schema_type='{4}'
                        and a.end_time < to_date('{5}','yyyy-mm-dd hh24:mi:ss')
                        union all
                        select count(1) as see_no
                        from fin_opr_booking b
                        where b.schema_no = '{6}' and b.valid_flag = '1'
                        and b.oper_date <= to_date('{7}','yyyy-mm-dd hh24:mi:ss')
                        --and b.confirm_opcd is not null
                        )";


        public static string GetSeeNoBySchemaNo = @"
 select sum(seeNo)+1 from (
 select sum(a.tel_lmt) + sum(a.reg_lmt) + sum(a.spe_lmt)as seeNo 
    from fin_opr_schema a
    join (select * from fin_opr_schema where id='{0}') b
         on b.see_date=a.see_date 
         and a.doct_code=b.doct_code
         and a.noon_code=b.noon_code 
         and a.schema_type=b.schema_type
         and a.end_time<b.end_time
 union  
 select sum(a.reged) + sum(a.tel_reging) + sum(a.spe_lmt) as seeNo 
    from fin_opr_schema a
    join (select * from fin_opr_schema where id='{0}') b
          on b.see_date=a.see_date 
          and a.doct_code=b.doct_code
          and a.noon_code=b.noon_code 
          and a.schema_type=b.schema_type
          and a.end_time=b.end_time
          )";


        /// <summary>
        /// 获取合同单位信息
        /// </summary>
        public static string GetPactInfo = @"select pact_code,
       pact_name,
       paykind_code,
       pub_ratio,
       pay_ratio,
       own_ratio,
       eco_ratio,
       arr_ratio,
       baby_flag,
       mcard_flag,
       control_flag,
       flag,
       day_limit,
       month_limit,
       year_limit,
       once_limit,
       PRICE_FORM,
       BED_LIMIT,
       AIR_LIMIT,
       SORT_ID,
       SIMPLE_NAME,
       dll_name,
       dll_description,
       pactsystype,
       fun_get_querycode(pact_name, 1) as spellCode,
       fun_get_querycode(pact_name, 0) as wbCode,
       patient_type_id,
       patient_type_name,
       outpatientfee_flag
  from fin_com_pactunitinfo p
 where pact_code = '{0}'";

        /// <summary>
        /// 获取门诊流水号
        /// </summary>
        public static string GetClinicCode = @"select seq_fin_clinicno.nextval from dual";

        /// <summary>
        /// 获取患者基本信息
        /// </summary>
        public static string GetPatientInfo = @"SELECT a.card_no,
                                                   a.name, --姓名
                                                   a.birthday, --出生日期
                                                   a.sex_code, --性别
                                                   a.idenno, --身份证号
                                                   a.mcard_no, --医疗证号
                                                   a.home_tel, --电话
                                                   a.home --地址
                                              FROM com_patientinfo a
                                              left join fin_opb_accountcard b --病人基本信息表
                                                on a.card_no = b.card_no
                                               where (a.card_no = '{0}' or b.markno = '{0}' or a.idenno='{0}')
                                               and rownum=1
                                                ";

        /// <summary>
        /// 取预约信息
        /// </summary>
        public static string GetBookingInfo = @"select a.clinic_code, a.schema_no, a.doct_code,a.doct_name,a.dept_code,a.dept_name ,trunc(a.booking_date)booking_date 
                            ,a.REGLEVL_CODE,a.begin_time ,a.source,a.see_flag ,a.oper_date,a.end_time from   fin_opr_booking a-- join fin_opb_accountcard b  on a.card_no=b.card_no
                        where (a.card_no='{0}' or a.idenno ='{2}')
                        and  trunc(a.booking_date)=trunc(sysdate) 
                        and a.valid_flag='1'
                        and a.schema_no='{1}'
                        and rownum=1";


        /// <summary>
        /// 预约取号时更新预约状态
        /// </summary>
        public static string UpdateBookInfo = @"update fin_opr_booking a
                        set a.oper_code='" +ZZSB.RegisterManager.OPERID+ @"',
                        a.confirm_opcd='" + ZZSB.RegisterManager.OPERID + @"',
                        a.confirm_date=sysdate,
                        a.see_flag='1',
                        a.reg_id='{0}'
                        where a.clinic_code='{1}'";
        

        /// <summary>
        /// 获取挂号等级费用
        /// </summary>
        public static string GetRegFee = @"SELECT  
                                             ID,  --0                                   --流水号
                                             PACT_CODE,                              --合同单位
                                             REGLEVL_CODE,                           --挂号级别
                                             DEPT_CODE,                              --适用范围 ALL全院,其余为特殊科室代码
                                             REG_FEE,  --4                              --挂号费
                                             CHCK_FEE,                               --检查费
                                             DIAG_FEE,   --6                            --诊察费
                                             OTH_FEE,                                --附加费
                                             OPER_CODE,                              --操作员
                                             OPER_DATE,                               --操作时间
                                             DIAG_PUBFEE
                                            FROM fin_opr_regfeeonpact  WHERE pact_code = '{0}' and reglevl_code = '{1}'";


        /// <summary>
        /// 获取护士分诊队列表
        /// </summary>
        public static string GetNurQueueByDept = @"SELECT nurse_cell_code, --门诊护士站代码0
                                                           queue_code, --队列代码1
                                                           queue_name, --队列名称2
                                                           noon_code, --午别
                                                           queue_flag, --1医生队列/2自定义队列
                                                           sort_id, --显示顺序
                                                           valid_flag, --1有效/0无效
                                                           remark, --备注
                                                           oper_code, --操作员
                                                           oper_date, --操作时间
                                                           queue_date, --队列日期
                                                           doct_code, --看诊医生
                                                           ROOM_ID,
                                                           ROOM_NAME,
                                                           CONSOLE_CODE,
                                                           CONSOLE_NAME,
                                                           EXPERT_FLAG,
                                                           dept_code,
                                                           dept_name,
                                                           waiting_count
                                                      FROM met_nuo_queue --门诊护士站分诊队列表
 where queue_code=(
                                                     select a.queuecode from fin_opr_queue a 
                                                     where a.schemano='{0}' and rownum=1)
                                                     and valid_flag='1' 
                                                
                                                    ";

        /*   where trunc(queue_date) =
                                                           trunc(to_date('{0}', 'yyyy-mm-dd hh24:mi:ss')) --队列日期
                                                       and dept_code = '{1}' --科室
                                                       and noon_code = '{2}' --午别
                                                       and valid_flag = fun_get_valid
                                                       and met_nuo_queue.nurse_cell_code is not null
                                                       and met_nuo_queue.expert_flag != '1' --非专家号才自动分诊
                                                       and met_nuo_queue.room_id='{3}'
                                                     order by (select count(d.clinic_code)
                                                                 from met_nuo_assignrecord d
                                                                where d.queue_code = met_nuo_queue.queue_code
                                                                  and d.assign_flag = '1')
,
                                                              waiting_count   */

        /// <summary>
        /// 获取护士分诊队列表
        /// </summary>
        public static string GetNurQueueByDoct = @"SELECT nurse_cell_code, --门诊护士站代码
                                                           queue_code, --队列代码
                                                           queue_name, --队列名称
                                                           noon_code, --午别
                                                           queue_flag, --1医生队列/2自定义队列
                                                           sort_id, --显示顺序
                                                           valid_flag, --1有效/0无效
                                                           remark, --备注
                                                           oper_code, --操作员
                                                           oper_date, --操作时间
                                                           queue_date, --队列日期
                                                           doct_code, --看诊医生
                                                           ROOM_ID,
                                                           ROOM_NAME,
                                                           CONSOLE_CODE,
                                                           CONSOLE_NAME,
                                                           EXPERT_FLAG,
                                                           dept_code,
                                                           dept_name,
                                                           waiting_count
                                                      FROM met_nuo_queue --门诊护士站分诊队列表

                                                     where doct_code = '{1}'
                                                       and trunc(queue_date) =
                                                           trunc(to_date('{0}', 'yyyy-mm-dd hh24:mi:ss'))
                                                       and noon_code = '{2}'
                                                       and valid_flag = fun_get_valid
                                                    ";


        /// <summary>
        /// 获取fin_com_invoice发票信息
        /// </summary>
        public static string GetInvoiceInfoUsed = @"select t.start_no, t.end_no, t.used_no
                                                  from fin_com_invoice t
                                                 where t.get_person_code = '{0}'
                                                   and t.invoice_kind = 'R'
                                                   and t.used_state = '{1}'
                                                ";

        /// <summary>
        /// 获取INVOICE-R发票信息
        /// </summary>
        public static string GetInvoiceR = @"select name,
                                                   mark
                                              from com_dictionary
                                             where type = 'INVOICE-R'
                                               and code = '{0}'
                                               and VALID_STATE = fun_get_valid
                                                ";

        /// <summary>
        /// 获取InvoiceUserCode发票信息
        /// </summary>
        public static string GetInvoiceUserCode = @"select name
                                              from com_dictionary
                                             where type = 'InvoiceUserCode'
                                               and code = '{0}'
                                               and VALID_STATE = fun_get_valid
                                                ";

        /// <summary>
        /// 获取排班信息
        /// </summary>
        public static string GetSchema = @"SELECT id, --序号
                                                   schema_type, --排班类型，0科室/1医生
                                                   see_date, --看诊日期
                                                   week, --星期
                                                   noon_code, --午别
                                                   dept_code, --科室代号
                                                   dept_name, --科室名称
                                                   doct_code, --医师代号
                                                   doct_name, --医生姓名8
                                                   doct_type, --1在职/2返聘
                                                   reg_lmt, --挂号限额10
                                                   reged, --挂号已挂
                                                   valid_flag, --1正常/0停诊
                                                   reason_no, --停诊原因
                                                   reason_name, --停诊原因名称
                                                   stop_opcd, --停止人15
                                                   stop_date, --停止时间
                                                   remark, --备注
                                                   oper_code, --操作员
                                                   oper_date, --最近改动日期
                                                   begin_time,--20
                                                   end_time,
                                                   tel_lmt,
                                                   tel_reged,
                                                   tel_reging,
                                                   spe_lmt,--25
                                                   spe_reged,
                                                   append_flag,
                                                   order_no,--28
                                                   reglevl_code,
                                                   reglevl_name,--30
                                                   ROOM_ID, --诊室代码
                                                   ROOM_NAME, --诊室名称
                                                   CONSOLE_CODE, --诊台代码
                                                   CONSOLE_NAME --诊台名称
                                              FROM fin_opr_schema --医师出诊表
                                             WHERE id = '{0}'
                                            ";

        /// <summary>
        /// 插入挂号主表
        /// </summary>
        public static string insertReg = @"INSERT INTO fin_opr_register --挂号主表
                                          (clinic_code, --门诊号/发票号
                                           card_no, --就诊卡号
                                           reg_date, --挂号日期
                                           noon_code, --午别
                                           name, --姓名
                                           idenno, --身份证号
                                           sex_code, --性别
                                           birthday, --出生日
                                           paykind_code, --结算类别号
                                           paykind_name, --结算类别名称
                                           pact_code, --合同号
                                           pact_name, --合同单位名称
                                           mcard_no, --医疗证号
                                           reglevl_code, --挂号级别
                                           reglevl_name, --挂号级别名称
                                           dept_code, --科室号
                                           dept_name, --科室名称
                                           seeno, --看诊序号
                                           doct_code, --医师代号
                                           doct_name, --医师姓名
                                           see_date, --看诊日期
                                           ynregchrg, --挂号收费标志
                                           ynbook, --是否预约
                                           ynfr, --1初诊/2复诊
                                           reg_fee, --挂号费
                                           chck_fee, --检查费
                                           diag_fee, --诊察费
                                           oth_fee, --附加费
                                           own_cost, --自费金额
                                           pub_cost, --报销金额
                                           pay_cost, --自付金额
                                           valid_flag, --退号标志
                                           oper_code, --操作员代码
                                           ynsee, --是否看诊
                                           check_flag, --1未核查/2已核查
                                           rela_phone, --联系电话
                                           address, --地址
                                           trans_type, --交易类型
                                           card_type, --证件类型
                                           begin_time, --开始时间段
                                           end_time, --结束时间段
                                           cancel_opcd, --作废人
                                           cancel_date, --作废时间
                                           invoice_no,--发票号
                                           recipe_no,--处方号
                                           append_flag,--是否加号
                                           order_no,--每日顺序号
                                           schema_no,--排班序号
                                           oper_date, --操作时间
                                           in_source,--患者来源
                                           is_sendinhoscase,--1：需要提取病案0：不需要提取病案
                                           IS_ENCRYPTNAME,--是否加密姓名
                                           normalname,--密文
                                           eco_cost,--优惠金额
                                           IS_Account,--账户流程标识1 账户挂号 0普通
                                           Is_Emergency,--是否急诊号
                                           mark1,--扩展字段1
                                           current_card,--56当前使用卡号
                                           current_cardtype,--57当前使用卡类型
                                           in_times,--58登记次数
                                           PATIENT_TYPE,--患者类别（普通、VIP、特诊等） 常数PersonType
                                           reg_no,--诊金登记单号
                                           reg_diag_fee,--诊金金额
                                           reg_diag_code, --诊金代码
                                           triage_flag,   --分诊标志,0未分/1已分
                                           triage_opcd,   --分诊护士代码
                                           triage_date,
                                           hos_code,
                                           triage_serialnum,
                                           InformedConsentResult
                                           )
                                        VALUES
                                          ('{0}', --门诊号/发票号
                                           '{1}', --就诊卡号
                                           to_date('{2}', 'yyyy-mm-dd HH24:mi:ss'), --挂号日期
                                           '{3}', --午别
                                           '{4}', --姓名
                                           '{5}', --身份证号
                                           '{6}', --性别
                                           to_date('{7}', 'yyyy-mm-dd HH24:mi:ss'), --出生日
                                           '{8}', --结算类别号
                                           '{9}', --结算类别名称
                                           '{10}', --合同号
                                           '{11}', --合同单位名称
                                           '{12}', --医疗证号
                                           '{13}', --挂号级别
                                           '{14}', --挂号级别名称
                                           '{15}', --科室号
                                           '{16}', --科室名称
                                           '{17}', --看诊序号
                                           '{18}', --医师代号
                                           '{19}', --医师姓名
                                           null, --看诊日期
                                           '{20}', --挂号收费标志
                                           '{21}', --是否预约
                                           '{22}', --1初诊/2复诊
                                           '{23}', --挂号费
                                           '{24}', --检查费
                                           '{25}', --诊察费
                                           '{26}', --附加费
                                           '{27}', --自费金额
                                           '{28}', --报销金额
                                           '{29}', --自付金额
                                           '{30}', --有效标志
                                           '{31}', --操作员代码
                                           '{32}', --是否看诊
                                           '{33}', --1未核查/2已核查
                                           '{34}', --联系电话
                                           '{35}', --地址
                                           '{36}', --交易类型
                                           '{37}', --证件类型
                                           to_date('{38}', 'yyyy-mm-dd HH24:mi:ss'), --开始时间
                                           to_date('{39}', 'yyyy-mm-dd HH24:mi:ss'), --开始时间
                                           '{40}', --作废人
                                           to_date('{41}', 'yyyy-mm-dd hh24:mi:ss'),--作废时间
                                           '{42}',--发票号
                                           '{43}',--处方号
                                           '{44}',--是否加号
                                           '{45}',--每日顺序号
                                           '{46}',--排班序号
                                           to_date('{47}', 'yyyy-mm-dd hh24:mi:ss'),--操作时间
                                           '{48}',--患者来源
                                           '{49}',--1：需要提取病案0：不需要提取病案
                                           '{50}',--是否加密姓名
                                           '{51}',--密文
                                           '{52}',--优惠金额
                                           '{53}',--账户流程标识1 账户挂号 0普通
                                           '{54}',--是否急诊号
                                           '{55}',--扩展字段1
                                           '{56}',--56当前使用卡号
                                           '{57}',--57当前使用卡类型
                                           {58},--58登记次数
                                           '{59}',--患者类别（普通、VIP、特诊等） 常数PersonType
                                           '{60}',--诊金登记单号
                                           {61},--诊金金额
                                           '{62}', --诊金代码
                                           '{63}', --分诊标志,0未分/1已分
                                           '{64}', --分诊护士代码
                                           to_date('{65}', 'yyyy-mm-dd HH24:mi:ss'), --分诊时间
                                            '{66}',--医院编号
                                            '{67}',--急诊分诊流水号
                                            '{68}'--知情同意书结果 0拒绝 1同意
                                           )";

        /// <summary>
        /// 插入挂号费用表
        /// </summary>
        public static string insertRegFee = @"insert into fin_opb_accountcardfee
                                              (invoice_no,--发票
                                               trans_type,--交易类型
                                               card_no,--门诊卡号
                                               markno,--医疗证号
                                               type,--身份标识卡类别 0无卡1磁卡 2IC卡
                                               tot_cost,--总额
                                               fee_oper,--收费人
                                               fee_date,--收费时间
                                               oper_code,--操作人
                                               oper_date,--操作时间
                                               balance_flag,--0未日结/1已日结
                                               balance_no,--日结标识号
                                               balance_opcd,--日结人
                                               balance_date,--日结时间
                                               cancel_flag,--‘0’ 无效 ‘1’ 有效,2退费
                                               print_invoiceno,--实际发票打印号码
                                               fee_type,--1=卡费用，2=病历本费用，3=挂号费，4=诊金，5=检查费，6=空调费
                                               clinic_no,--病历号/门诊号
                                               remark,--备注
                                               own_cost,--自费金额
                                               pub_cost,--报销金额
                                               pay_cost,--自付金额
                                               pay_type)--支付方式
                                            values
                                              ('{0}',
                                               '{1}',
                                               '{2}',
                                               nvl('{3}','0'),
                                               '{4}',
                                               {5},
                                               '{6}',
                                               to_date('{7}', 'yyyy-mm-dd hh24:mi:ss'),
                                               '{8}',
                                               to_date('{9}', 'yyyy-mm-dd hh24:mi:ss'),
                                               '{10}',
                                               '{11}',
                                               '{12}',
                                               to_date('{13}', 'yyyy-mm-dd hh24:mi:ss'),
                                               '{14}',
                                               '{15}',
                                               '{16}',
                                               '{17}',
                                               '{18}',
                                               {19},
                                               {20},
                                               {21},
                                               '{22}')";

        /// <summary>
        /// 插入护士分诊记录表
        /// </summary>
        public static string insertAssignRecord = @"INSERT INTO met_nuo_assignrecord   --护士分诊记录表
                                                      ( 
                                                        clinic_code,   --门诊号
                                                        see_sequence,   --看诊序号
                                                        card_no,   --病历号
                                                        reg_date,   --挂号日期
                                                        name,   --患者姓名
                                                        sex_code,   --性别
                                                        paykind_code,   --结算类别
                                                        ynurg,   --1急诊/0普通
                                                        ynbook,   --1预约/0普通
                                                        dept_code,   --看诊科室
                                                        dept_name,   --科室名称
                                                        queue_name,   --队列名称
                                                        room_id,   --出诊诊室
                                                        queue_code,   --队列代码
                                                        room_name,   --诊室名称
                                                        doct_code,   --看诊医生
                                                        see_date,   --看诊时间
                                                        assign_flag,   --1分诊/2进诊/3诊出
                                                        nurse_cell_code,   --分诊科室
                                                        triage_date,   --分诊时间
                                                        in_date,   --进诊时间
                                                        out_date,   --出诊时间
                                                        oper_code,   --操作员
                                                        oper_date,  --操作时间
                                                        console_code,--诊台代码
                                                        console_name,--诊台名称
                                                        reglvl_code,-- 挂号级别代码
                                                        reglvl_name,--挂号级别
                                                        order_no --每日顺序号
                                                        )
                                                 VALUES 
                                                      (  
                                                        '{0}',   --门诊号
                                                        '{1}',   --看诊序号
                                                        '{2}',   --病历号
                                                        to_date('{3}','yyyy-mm-dd HH24:mi:ss'),   --挂号日期
                                                        '{4}',   --患者姓名
                                                        '{5}',   --性别
                                                        '{6}',   --结算类别
                                                        '{7}',   --1急诊/0普通
                                                        '{8}',   --1预约/0普通
                                                        '{9}',   --看诊科室
                                                        '{10}',   --科室名称
                                                        '{11}',   --队列名称
                                                        '{12}',   --出诊诊室
                                                        '{13}',   --队列代码
                                                        '{14}',   --诊室名称
                                                        '{15}',   --看诊医生
                                                        to_date('{16}','yyyy-mm-dd HH24:mi:ss'),   --看诊时间
                                                        '{17}',   --1分诊/2进诊/3诊出
                                                        '{18}',   --分诊科室
                                                        to_date('{19}','yyyy-mm-dd HH24:mi:ss'),   --分诊时间
                                                        to_date('{20}','yyyy-mm-dd HH24:mi:ss'),   --进诊时间
                                                        to_date('{21}','yyyy-mm-dd HH24:mi:ss'),   --进诊时间
                                                        '{22}',   --操作员
                                                        to_date('{23}','yyyy-mm-dd HH24:mi:ss'),  --操作时间
                                                        '{24}', --诊台代码
                                                        '{25}', --诊台名称
                                                        '{26}',--挂号级别
                                                        '{27}',--挂号级别名称
                                                        '{28}' --每日顺序号
                                                    )";

        /// <summary>
        /// 更新护士分诊队列表
        /// </summary>
        public static string updateNurQueues = @"update met_nuo_queue
                                                        set waiting_count = waiting_count + '1'
                                                        where queue_code = '{0}' ";

        /// <summary>
        /// 字符加1
        /// </summary>
        public static string addnumber = @"select fun_get_addnumber('{0}') from  dual";

        /// <summary>
        /// 更新发票表
        /// </summary>
        public static string updateComInvoice = @"update fin_com_invoice t
                                                   set t.used_no = '{3}', t.used_state = '{4}'
                                                 where t.get_person_code = '{0}'
                                                   and t.start_no = '{1}'
                                                   and t.end_no = '{2}'
                                                   and t.invoice_kind = 'R'
                                                ";

        /// <summary>
        /// 更新发票表
        /// </summary>
        public static string updateComInvoiceNew = @"update fin_com_invoice t
                                                       set t.used_no = '{1}', t.used_state = '{2}'
                                                     where t.get_person_code = '{0}'
                                                       and t.get_dtime = to_date('{3}','yyyy-mm-dd hh24:mi:ss')";

        /// <summary>
        /// 更新发票表
        /// </summary>
        public static string GetUnUseInvoce = @"select i.get_dtime, i.start_no, i.end_no
                                                from fin_com_invoice i
                                               where i.get_person_code = '{0}'
                                                 and i.used_state = '0'
                                                 and i.invoice_kind = 'R'
                                                 and rownum = 1)";

        /// <summary>
        /// 更新发票表
        /// </summary>
        public static string updatecomDictionary = @"update COM_DICTIONARY
                                                       set name        = '{1}',
                                                           mark        = '{2}',
                                                           VALID_STATE = '1',
                                                           oper_code   = '009999',
                                                           oper_Date   = sysdate
                                                     where type = 'INVOICE-R'
                                                       and code = '{0}'";

  
        public static string InsertSIRegister = @" INSERT INTO  FIN_OPB_ZHUHAISIREGINFO ZH
                                (
                                IDNO,
                                NAME,
                                RECIPENO,
                                REG_ITEM_CODE,
                                TOTCOST,
                                YIGAIACCOUNT,
                                BZACCOUNT,
                                PAYACCOUNT,
                                OPERDATE,
                                ASSURANCE,
                                MTRECIPENO,
                                QUERY_DATE
                                )
                                 Values
                                ( 
                                '{0}',          '{1}',        '{2}',        '{3}', 
                                '{4}',          '{5}',        '{6}',        '{7}', 
                                '{8}',          '{9}',        '{10}',       '{11}'
                                 )";
        /// <summary>
        /// 对账信息
        /// </summary>
        public static string InsertCheck = @"insert into FIN_OPB_OUTPATIENTCHECK(
CLINIC_CODE,
CHECK_TYPE,
TRANS_TYPE,
INVOICE_NO,
BANKCARDNO,
VOUCHNO,
MARK,
TOT_COST,
OPER_CODE,
OPER_DATE)
values (
'{0}',
'{1}',
'{2}',
'{3}',
'{4}',
'{5}',
'{6}',
'{7}',
'{8}',
sysdate)";


        /// <summary>
        /// 获取看诊序号(新)
        /// </summary>
        public static string GetNewSeeNo = @"
                select 
                fos.current_value+1
                from fin_opr_seesequence fos
                where fos.see_date=to_date('{0}','yyyy-mm-dd HH24:mi:ss')
                and fos.see_subject='{1}'
                and fos.see_type='{2}'
                and fos.noon_code='{3}'";

        /// <summary>
        /// 更新看诊序号(加1)
        /// </summary>
        public static string UpdateSeeNo = @"
                 update fin_opr_seesequence fos 
                 set fos.current_value=fos.current_value+1
                 where fos.see_date=to_date('{0}','yyyy-mm-dd HH24:mi:ss')
                 and fos.see_subject='{1}'
                 and fos.see_type='{2}'
                 and fos.noon_code='{3}'";
        public static string SetSeeNo = @"
                 UPDATE fin_opr_seesequence   --挂号看诊序号表
                 SET current_value={4}    --当前序号
                 WHERE see_date=to_date('{0}','yyyy-mm-dd HH24:mi:ss')
                 AND see_type='{1}'
                 AND see_subject='{2}'
                 AND nvl(noon_code,' ')=nvl('{3}',' ')
                 ";

        /// <summary>
        /// 插入省集中平台医保主表
        /// </summary>
        public static string InsertGDSIinfo = @"INSERT INTO FIN_IPR_SIINMAININFO_GD f
                                (
                                INPATIENT_NO,
                                REG_NO,
                                BALANCE_NO,
                                INVOICE_NO,
                                CARD_NO,
                                MCARD_NO,
                                NAME,
                                IDENNO,
                                CLINIC_DIAGNOSE,
                                PAYKIND_CODE,
                                PACT_CODE,
                                PACT_NAME,
                                OPER_CODE,
                                OPER_DATE,
                                TOT_COST,
                                PUB_COST,
                                OWN_COST,
                                VALID_FLAG,
                                FEE_TIMES,
                                SEX_CODE,
                                DEPT_CODE,
                                IN_DATE,
                                BALANCE_DATE,
                                TYPE_CODE, --22
                                BKA825,
                                BKA826,
                                AKA151,
                                BKA838,
                                AKB067, --27
                                AKB066,
                                BKA821,
                                BKA839,
                                AKE039,
                                AKE035,
                                AKE026,
                                AKE029,
                                BKA841,
                                BKA842,
                                BKA840,   --37 
                                PATIENT_NO,
                                AAA027,
                                AAZ267,
                                bka438,
                                aab301,
                                bka006,
                                aae140,
                                aka130,
                                DEPT_NAME
                                )
                                Values
                                (
                                '{0}',
                                '{1}',
                                '{2}',
                                '{3}',
                                '{4}',
                                '{5}',
                                '{6}',
                                '{7}',
                                '{8}',
                                '{9}',
                                '{10}',
                                '{11}',
                                '{12}',
                                to_date('{13}','YYYY-MM-DD hh24:mi:ss'),
                                '{14}',
                                '{15}',
                                '{16}',
                                '{17}',
                                0,
                                '{18}',
                                '{19}',
                                to_date('{20}','YYYY-MM-DD hh24:mi:ss'),
                                to_date('{21}','YYYY-MM-DD hh24:mi:ss'),
                                '{22}',
                                '{23}',
                                '{24}',
                                '{25}',
                                '{26}',
                                '{27}',
                                '{28}',
                                '{29}',
                                '{30}',
                                '{31}',
                                '{32}',
                                '{33}',
                                '{34}',
                                '{35}',
                                '{36}',
                                '{37}',
                                '{38}',
                                '{39}',
                                '{40}',
                                '{41}',
                                '{42}',
                                '{43}',
                                '{44}',
                                 '{45}',
                                 '{46}'
                                )";
        #endregion

        #region 排班

        #region 出诊科室

       public static string QueryBookDeptSql = @"select 
        t.id regsourceid,--排班编号,
        null regsourcename,--排班名称
        decode(t.schema_type,'1',2,'0',1,0) schematype,--排班类型
        t.reglevl_code typecode,--号类编号
        t.reglevl_name typename,--号类描述
        t.dept_code deptcode,--科室编号
        t.dept_name deptname,--科室名称
        t.doct_code doctorcode,--医生编号
        nvl(t.doct_name,'普通号') doctorname,--医生姓名
        (select y.remark from com_employee y where y.empl_code=t.doct_code)  specify,--医生专长
        (select y.levl_code from com_employee y where y.empl_code=t.doct_code)  rankid,--医生级别编号
        fun_get_levelname((select y.levl_code from com_employee y where y.empl_code=t.doct_code)) rankname,--医生级别名称
        t.begin_time starttime,--开始时间
        t.end_time endtime,--结束时间
        t.noon_code sessioncode,--出诊时段编号
        decode(t.noon_code,'1','上午','2','下午','3','晚上',0) sessionname,--出诊时段名称
        t.tel_lmt allcount, --全部号源数
        t.tel_reging outcount,--已挂号数
        t.tel_lmt-t.tel_reging havecount,--剩余号源数
        --tt.reg_fee+tt.chck_fee+tt.diag_fee+tt.oth_fee totalregfee,--总挂号费 a.reg_fee+a.diag_fee
        tt.reg_fee+tt.diag_fee totalregfee,--总挂号费 a.reg_fee+a.diag_fee
        tt.reg_fee regfee,--挂号费
        tt.chck_fee treatfee,--检查费
        tt.diag_fee servicefee,--服务费
        null metafee,--材料费
        tt.oth_fee otherfee,--其它费用
        t.room_name||(select y.remark from MET_NUO_CONSOLE y where t.room_id=y.room_code and rownum='1') admitaddress,--候诊地点
        null note, --备用
(SELECT decode(count(1),0,'0','1') FROM com_dictionary d where d.type = 'ELDERLYVOUCHERDOCTOR' and d.valid_state = '1' AND d.code = t.doct_code) elderlyVoucherDoctorFlag
  from FIN_OPR_SCHEMA t,fin_opr_regfeeonpact tt
  where nvl(t.reglevl_code,1) = tt.reglevl_code
  and tt.pact_code='1'
  and t.id in(select t.id from FIN_OPR_SCHEMA t where t.dept_code not in('6068','7021') 
  and t.stop<>'1'
 -- and t.doct_code<>'850201'
 -- and t.room_name not like'%临时%'
 -- and t.end_time>=sysdate
  and t.valid_flag='1' 
  and t.see_date=trunc(to_date('{0}','yyyy-mm-dd hh24:mi:ss'))
  and t.dept_code='{1}'
  and (t.tel_lmt-t.tel_reging)>0
  --and fun_get_noon(sysdate)=t.noon_code  group by t.doct_code
   )
   order by (select j.sort_id from com_employee j where j.empl_code=t.doct_code and j.valid_state='1'),
  t.begin_time ";

        #endregion

        #region 取医生所有时段

        public static string QueryDoctTimes = @"  
  select a.reglevl_code as SchemaType,
         a.begin_time as StartTime,
         a.end_time as EndTime,
         a.id as SessionCode,
         '' as SessionName,
         a.tel_lmt as AllCount,
         a.tel_reging as OutCount,
         a.tel_lmt - a.tel_reging as HaveCount,
         b.reg_fee + b.diag_fee as TotalRegFee,
         a.reglevl_name as Note
    from fin_opr_schema a
    join fin_opr_regfeeonpact b
      on nvl(a.reglevl_code, 1) = b.reglevl_code
   where a.see_date = trunc(to_date('{0}', 'yyyy-mm-dd hh24:mi:ss'))
     and a.tel_lmt > a.tel_reging
     and a.dept_code = '{1}'
     and a.doct_code = '{2}'
     and b.pact_code='1'
     and a.reglevl_code<>'4'
";

        #endregion

        public static string BookLockSql = @"
update fin_opr_schema s --医师出诊表
   set s.tel_reged  = s.tel_reged + {1}, --挂号已挂
       s.tel_reging = s.tel_reging + {1},
       s.order_no   = nvl(s.order_no, 0) + 1
 where s.id = '{0}'
   and s.tel_lmt > s.tel_reging
   and s.valid_flag='1'
   and s.stop<>'1'" ;

        #endregion

        #region 预约

       public static string BookInsertSql = @"
                        insert into fin_opr_booking
                        (
                        CLINIC_CODE,--0
                        CARD_NO,--1
                        BOOKING_DATE,--2
                        NOON_CODE,--3
                        NAME,--4
                        IDENNO,--5
                        SEX_CODE,--6
                        BIRTHDAY,--7
                        RELA_PHONE,--8
                        ADDRESS,--9
                        SCHEMA_NO,--10
                        DEPT_CODE,--11
                        DEPT_NAME,--12
                        BEGIN_TIME,--13
                        END_TIME,--14
                        DOCT_CODE,--15
                        DOCT_NAME,--16
                        SEE_FLAG,--17
                        APP_FLAG,--18
                        OPER_CODE,--19
                        OPER_DATE,--20
                        CONFIRM_OPCD,--21
                        CONFIRM_DATE,--22
                        REGLEVL_CODE,--23
                        VALID_FLAG,--24
                        REG_ID, --25
                        SOURCE
                        )
                        select 
                        {2},--CLINIC_CODE,--0
                        p.card_no,-- CARD_NO,--1
                        s.begin_time,-- BOOKING_DATE,--2
                        s.noon_code,-- NOON_CODE,--3
                        p.name,-- NAME,--4
                        p.idenno,-- IDENNO,--5
                        p.sex_code,-- SEX_CODE,--6
                        to_date('{3}','yyyy-mm-dd hh24:mi:ss') birthday,--BIRTHDAY,--7
                        p.home_tel ,--RELA_PHONE,--8
                        p.home,--ADDRESS,--9
                        s.id,--SCHEMA_NO,--10
                        s.dept_code,--DEPT_CODE,--11
                        s.dept_name,--DEPT_NAME,--12
                        s.begin_time,--BEGIN_TIME,--13
                        s.end_time,--END_TIME,--14
                        s.doct_code,--DOCT_CODE,--15
                        s.doct_name,--DOCT_NAME,--16
                        '1',--SEE_FLAG,--17
                        '0',--APP_FLAG,--18
                        '{4}' OPER_CODE,--19
                        sysdate,-- OPER_DATE,--20
                        '{7}',--CONFIRM_OPCD,--21
                        {8},--CONFIRM_DATE,--22
                        s.reglevl_code,-- REGLEVL_CODE,--23
                        '1',-- VALID_FLAG,--24
                        '{5}' REG_ID, --25
                        '{6}' SOURCE --26
                        from fin_opr_schema s,com_patientinfo p
                        where s.id='{0}' 
                        and p.card_no='{1}'";

        #endregion

       public static string InsertTradeRecords = @"insert into FIN_OPB_TRADERECORDSZZSB
  (TRANSERNO, --交易流水号
   INVOICE_NO, --发票号
   CLINIC_NO, --流水号
   CARDNO, --卡号
   NAME, --姓名
   ORDERID, --订单号
   PAY_TYPE, --支付方式
   TYPE, --交易类型
   TOT_COST, --交易金额
   DEVICEID, --设备号
   OPER_DATE, --操作日期
   REMARK, --备注
   PACTCODE --合同单位
   )
values
  ('{0}', --交易流水号
   '{1}', --发票号
   '{2}', --流水号
   '{3}', --卡号
   '{4}', --姓名
   '{5}', --订单号
   '{6}', --支付方式
   '{7}', --交易类型
   '{8}', --交易金额
   '{9}', --设备号
   to_date('{10}', 'yyyy-mm-dd hh24:mi:ss'), --操作日期
   '{11}', --备注
   '{12}' --合同单位
   )
";
       /// <summary>
       /// 获取儿科科室，14周岁以上不可挂
       /// </summary>
       public static string GetPediatricsDeptCodeList = @"select to_char(wm_concat(code)) from com_dictionary d where d.type ='PediatricsDeptCodeList'  and d.valid_state = '1'";
       /// <summary>
       /// 获取14周岁以下不可挂的科室
       /// </summary>
       public static string Get14AgelimitDeptCodeList = @"select to_char(wm_concat(code)) from com_dictionary d where d.type ='Age14LimitDept'  and d.valid_state = '1'";
       
        /// <summary>
        /// 插入门诊费用表
        /// </summary>
       public static string insertRegFeeDetail = @"INSERT INTO FIN_OPB_FEEDETAIL
                     (RECIPE_NO,
                      SEQUENCE_NO,
                      TRANS_TYPE,
                      CLINIC_CODE,
                      CARD_NO,
                      REG_DATE,
                      REG_DPCD,
                      DOCT_CODE,
                      DOCT_DEPT,
                      ITEM_CODE,
                      ITEM_NAME,
                      DRUG_FLAG,
                      SPECS,
                      FEE_CODE,
                      CLASS_CODE,
                      UNIT_PRICE,
                      QTY,
                      DAYS,
                      INJECT_NUMBER,
                      EMC_FLAG,
                      DOSE_ONCE,
                      PACK_QTY,
                      PRICE_UNIT,
                      PUB_COST,
                      PAY_COST,
                      OWN_COST,
                      EXEC_DPCD,
                      EXEC_DPNM,
                      MAIN_DRUG,
                      OPER_CODE,
                      OPER_DATE,
                      PAY_FLAG,
                      CANCEL_FLAG,
                      CONFIRM_FLAG,
                      PACT_UNIT_FLAG,
                      NOBACK_NUM,
                      CONFIRM_NUM,
                      CONFIRM_INJECT,
                      MO_ORDER,
                      OVER_COST,
                      EXCESS_COST,
                      DRUG_OWNCOST,
                      COST_SOURCE,
                      SUBJOB_FLAG,
                      ACCOUNT_FLAG,
                      DOCTINDEPT,
                      PAYKIND_CODE,
                      PACT_CODE,
                      OLD_UNIT_PRICE,
                      PACKAGE_QTY,
                      BELONG_DEPT,
                      HOS_CODE,
                      INVOICE_SEQ)
                   VALUES
                     ('{0}',
                      '{1}',
                      '{2}',
                      '{3}',
                      '{4}',
                      to_date('{5}','YYYY-MM-DD hh24:mi:ss'),
                      '{6}',
                      '{7}',
                      '{8}',
                      '{9}',
                      '{10}',
                      '{11}',
                      '{12}',
                      '{13}',
                      '{14}',
                      '{15}',
                      '{16}',
                      '{17}',
                      '{18}',
                      '{19}',
                      '{20}',
                      '{21}',
                      '{22}',
                      '{23}',
                      '{24}',
                      '{25}',
                      '{26}',
                      '{27}',
                      '{28}',
                      '{29}',
                      to_date('{30}','YYYY-MM-DD hh24:mi:ss'),
                      '{31}',
                      '{32}',
                      '{33}',
                      '{34}',
                      '{35}',
                      '{36}',
                      '{37}',
                      '{38}',
                      '{39}',
                      '{40}',
                      '{41}',
                      '{42}',
                      '{43}',
                      '{44}',
                      '{45}',
                      '{46}',
                      '{47}',
                      '{48}',
                      '{49}',
                      '{50}',
                      '{51}',
                      '{52}')";
    }
}
