-- Create table
create table FIN_DISCOUNT_FEE
(
  item_code     VARCHAR2(30),
  item_name     VARCHAR2(100),
  discount_rate VARCHAR2(30),
  topprice      VARCHAR2(30) default 0,
  discount_type VARCHAR2(20),
  valid_state   VARCHAR2(10) default 1
)
tablespace HISDATA
  pctfree 10
  initrans 1
  maxtrans 255
  storage
  (
    initial 64K
    next 8K
    minextents 1
    maxextents unlimited
  );
-- Add comments to the columns 
comment on column FIN_DISCOUNT_FEE.item_code
  is '项目编码';
comment on column FIN_DISCOUNT_FEE.item_name
  is '项目名称';
comment on column FIN_DISCOUNT_FEE.discount_rate
  is '折价比例/折价限额';
comment on column FIN_DISCOUNT_FEE.topprice
  is '最高价';
comment on column FIN_DISCOUNT_FEE.discount_type
  is '折价类型';
comment on column FIN_DISCOUNT_FEE.valid_state
  is '有效状态';
