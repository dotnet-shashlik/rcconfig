import { PageContainer } from '@ant-design/pro-layout';
import { Button, Table, Modal, Form, Input, Select, message } from 'antd';
import { Link, useRequest } from 'umi';
import { useState } from 'react';
import { userList, deleteUser, createUser } from '@/services/api/user';
import { roleList } from '@/services/api/role';


const { Column } = Table;

const formLayout = {
  labelCol: { span: 8 },
  wrapperCol: { span: 13 },
};

export default () => {

  const [showCreate, setShowCreate] = useState(false);
  const userListRequest = useRequest(userList);
  const reload = () => {
    userListRequest.run();
    setShowCreate(false);
    message.success('success');
  };
  const createUserRequest = useRequest(createUser, {
    manual: true, onSuccess: reload
  });
  const roleListRequest = useRequest(roleList);
  const deleteUserList = useRequest(deleteUser, {
    manual: true, fetchKey: (id: any) => id, onSuccess: reload
  });

  const onDelete = (userId: number) => {
    Modal.confirm({
      title: 'Confirm delete this user?',
      onOk: async () => {
        deleteUserList.run(userId);
      }
    });
  };
  const [form] = Form.useForm();
  const onCreateSubmit = () => {
    form.validateFields()
      .then((model: any) => {
        createUserRequest.run(model);
      });
  };
  return (
    <PageContainer>
      <div style={{ marginBottom: "5px", textAlign: "right" }}>
        <Button type="primary" onClick={() => setShowCreate(true)}>创建新用户</Button>
        <Button type="default" onClick={userListRequest.run}>刷新</Button>
      </div>
      <Table dataSource={userListRequest.data} loading={userListRequest.loading}>
        <Column title="ID" dataIndex="id" />
        <Column title="UserName" dataIndex="userName" />
        <Column title="Roles" dataIndex="rolesStr" />
        <Column title="Action" key="action"
          render={(te_xt: any, user: any) => (
            <span>
              <Button type="link" loading={deleteUserList.fetches[user.id]?.loading} onClick={() => { onDelete(user.id) }}>Delete</Button>
              <Link to={`/users/detail/${user.id}`}>Detail</Link>
            </span>
          )} />
      </Table>

      <Modal title="Create User" visible={showCreate} onOk={onCreateSubmit} onCancel={() => setShowCreate(false)}
        confirmLoading={createUserRequest.loading}>
        <Form form={form} style={{ top: 20 }} {...formLayout}>
          <Form.Item
            label="UserName"
            name="userName"
            rules={[{ required: true }, { max: 32 }]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            label="NickName"
            name="nickName"
            rules={[{ required: true }, { max: 32 }]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            label="Password"
            name="password"
            rules={[{ required: true }, { max: 32 }]}
          >
            <Input.Password />
          </Form.Item>
          <Form.Item
            label="ConfirmPassword"
            name="confirmPassword"
            rules={[
              { required: true },
              { max: 32 },
              ({ getFieldValue }) => ({
                validator(_, value) {
                  if (!value || getFieldValue('password') === value) {
                    return Promise.resolve();
                  }
                  return Promise.reject(new Error('The two passwords that you entered do not match!'));
                },
              })]}
          >
            <Input.Password />
          </Form.Item>
          <Form.Item
            label="Roles"
            name="roles"
            rules={[{ required: true }]}
          >
            <Select
              mode="multiple"
              size="middle"
              placeholder="Please select role"
              style={{ width: '100%' }}
            >
              {roleListRequest.data?.map((role: any) => {
                return (<Select.Option key={`ROLE_${role.name}`} value={role.name}>{role.name}</Select.Option>);
              })}
            </Select>
          </Form.Item>
          <Form.Item
            label="Remark"
            name="remark"
            rules={[{ max: 255 }]}
          >
            <Input.TextArea />
          </Form.Item>
        </Form>
      </Modal>
    </PageContainer>);
};