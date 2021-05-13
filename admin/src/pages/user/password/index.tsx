import React, { useState, useEffect } from 'react';
import styles from './index.less';
import { PageContainer } from '@ant-design/pro-layout';
import { Form, Input, Button, Checkbox } from 'antd';
import { FormattedMessage, getIntl, useIntl } from 'umi';
import { changePassword } from '@/services/api/account';

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

  const onFinish = async (model: FormModel) => {
    let res = await changePassword(model);
    console.log(res);
  };

  const [form] = Form.useForm();


  return (
    <PageContainer>
      <Form
        form={form}
        {...layout}
        name="basic"
        initialValues={{ remember: true }}
        onFinish={onFinish}
      >
        <Form.Item
          label={useIntl().formatMessage({ id: 'app.settings.oldPassword' })}
          name="oldPassword"
          rules={[{ required: true, message: 'Please inout Old Password!' }]}
        >
          <Input.Password />
        </Form.Item>

        <Form.Item
          label={useIntl().formatMessage({ id: 'app.settings.newPassword' })}
          name="newPassword"
          rules={[{ required: true, message: 'Please inout New Password!' }]}
        >
          <Input.Password />
        </Form.Item>
        <Form.Item
          label={useIntl().formatMessage({ id: 'app.settings.confirmPassword' })}
          name="confirmPassword"
          rules={[{ required: true, message: 'Please inout Confirm Password!' }]}
        >
          <Input.Password />
        </Form.Item>

        <Form.Item {...tailLayout}>
          <Button type="primary" htmlType="submit">
            <FormattedMessage id="app.settings.submit" />
          </Button>
        </Form.Item>
      </Form>
    </PageContainer>);
};