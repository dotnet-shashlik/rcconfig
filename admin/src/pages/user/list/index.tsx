import React, { useState, useEffect } from 'react';
import styles from './index.less';
import { PageContainer } from '@ant-design/pro-layout';
import { Form, Input, Button, Checkbox, message, Table, Space } from 'antd';
import { FormattedMessage, getIntl, useIntl } from 'umi';
import { userList, getUserById } from '@/services/api/user';
import { clearAccessToken } from "@/utils/utils";

const { Column } = Table;

const layout = {
  labelCol: { span: 8 },
  wrapperCol: { span: 8 },
};
const tailLayout = {
  wrapperCol: { offset: 8, span: 16 },
};


interface FormModel {
  oldPassword?: string;
  newPassword?: string;
  confirmPassword?: string;
};

export default () => {

  const [list, setList] = useState([]);
  const [loading, setLoading] = useState<boolean>(false);

  const doSearch = async () => {
    setLoading(true);
    const res = await userList();
    if (res.success) {
      setList(res.data);
    }
    else
      message.error(res.msg);
    setLoading(false);
  };


  useEffect(() => {
    doSearch();
  }, [true]);

  return (
    <PageContainer>
      <Table dataSource={list} loading={loading}>
        <Column title="ID" dataIndex="id" />
        <Column title="UserName" dataIndex="userName" />
        <Column title="Action" key="action"
          render={(text, record) => (
            <Space size="middle">
              <a>Delete</a>
              <a>Detail</a>
            </Space>
          )} />
      </Table>
    </PageContainer>);
};