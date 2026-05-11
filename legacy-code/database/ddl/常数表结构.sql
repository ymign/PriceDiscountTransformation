-- Create table
create table COM_DICTIONARY
(
  type         VARCHAR2(40) not null,
  code         VARCHAR2(500) not null,
  name         VARCHAR2(4000),
  mark         VARCHAR2(4000),
  spell_code   VARCHAR2(40),
  wb_code      VARCHAR2(100),
  input_code   VARCHAR2(80),
  sort_id      NUMBER,
  valid_state  VARCHAR2(1) not null,
  oper_code    VARCHAR2(6),
  oper_date    DATE,
  is_common    VARCHAR2(1) default 1,
  kind_id      VARCHAR2(100),
  parent_code  VARCHAR2(14) default 'ROOT' not null,
  current_code VARCHAR2(14) default 'ROOT' not null
)
tablespace HISDATA
  pctfree 10
  initrans 1
  maxtrans 255
  storage
  (
    initial 64K
    next 1M
    minextents 1
    maxextents unlimited
  );
-- Add comments to the table 
comment on table COM_DICTIONARY
  is '公共_常数表';
-- Add comments to the columns 
comment on column COM_DICTIONARY.type
  is '常数类型 银行BANK 床等级BEDGRADE 减免类型DERATEFEETYPE  国家COUNTRY,民族NATION,职业PROFESSION 关系RELATIVE 职务POSITION 政治面貌POLITICS 职级LEVEL 地区ARAE 员工状态EMPLSTATUS 学历EDUCATION 用法USAGE 使用方式USWAY
                					挂号级别（REGTYPE),籍贯(DIST),
                					转归(ZG)';
comment on column COM_DICTIONARY.code
  is '编码';
comment on column COM_DICTIONARY.name
  is '名称';
comment on column COM_DICTIONARY.mark
  is '备注';
comment on column COM_DICTIONARY.spell_code
  is '拼音码';
comment on column COM_DICTIONARY.wb_code
  is '五笔';
comment on column COM_DICTIONARY.input_code
  is '输入';
comment on column COM_DICTIONARY.sort_id
  is '顺序号';
comment on column COM_DICTIONARY.valid_state
  is '有效性标志 0 在用 1 停用 2 废弃';
comment on column COM_DICTIONARY.oper_code
  is '操作员';
comment on column COM_DICTIONARY.oper_date
  is '操作时间';
comment on column COM_DICTIONARY.is_common
  is '是否常用';
comment on column COM_DICTIONARY.parent_code
  is '父级医疗机构编码';
comment on column COM_DICTIONARY.current_code
  is '本机医疗机构编码';
-- Create/Recreate indexes 
create index IDX_COM_DICTIONARY on COM_DICTIONARY (CODE)
  tablespace HISINDEX
  pctfree 10
  initrans 6
  maxtrans 255
  storage
  (
    initial 3M
    next 1M
    minextents 1
    maxextents unlimited
  );
create index IDX_COM_DICTIONARY01 on COM_DICTIONARY (NAME, TYPE)
  tablespace HISINDEX
  pctfree 10
  initrans 6
  maxtrans 165
  storage
  (
    initial 4M
    next 1M
    minextents 1
    maxextents unlimited
  );
-- Create/Recreate primary, unique and foreign key constraints 
alter table COM_DICTIONARY
  add constraint PK_COM_DICTIONARY primary key (TYPE, CODE, PARENT_CODE, CURRENT_CODE)
  using index 
  tablespace HISINDEX
  pctfree 10
  initrans 2
  maxtrans 255
  storage
  (
    initial 5M
    next 1M
    minextents 1
    maxextents unlimited
  );
-- Grant/Revoke object privileges 
grant select on COM_DICTIONARY to CHPS with grant option;
grant select on COM_DICTIONARY to EMR with grant option;
grant select on COM_DICTIONARY to EMR_CLIENT;
grant insert, update, delete on COM_DICTIONARY to HIP;
grant select on COM_DICTIONARY to HIP with grant option;
grant select on COM_DICTIONARY to IHOSP_CLIENT;
grant select on COM_DICTIONARY to READONLY_USER;
grant select on COM_DICTIONARY to ZWCD with grant option;
grant select on COM_DICTIONARY to ZWHL;
