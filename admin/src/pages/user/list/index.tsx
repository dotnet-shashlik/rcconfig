import { PageContainer } from '@ant-design/pro-layout';
import { Button, Table, Modal, Form, Input, Select, message } from 'antd';
import { Link, useRequest } from 'umi';
import { useState } from 'react';
import { userList, deleteUser, createUser, updateUser, getUserById } from '@/services/api/user';
import { roleList } from '@/services/api/role';


const { Column } = Table;

const formLayout = {
  labelCol: { span: 8 },
  wrapperCol: { span: 13 },
};

export default () => {

  const [showCreate, setShowCreate] = useState(false);
  const [showEdit, setshowEdit] = useState(false);
  const userListRequest = useRequest(userList);
  const getUserByIdRequest = useRequest(getUserById, { manual: true, fetchKey: (userId: number) => userId.toString() });
  const reload = () => {
    userListRequest.run();
    setShowCreate(false);
    setshowEdit(false);
    message.success('success');
  };
  const createUserRequest = useRequest(createUser, {
    manual: true, onSuccess: reload
  });
  const updateUserRequest = useRequest(updateUser, {
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
  const [editForm] = Form.useForm();

  const onShowEdit = async (userId: number) => {
    const user = await getUserByIdRequest.run(userId);
    editForm.setFieldsValue(user);
    setshowEdit(true);
  }

  const getRoleOptions = (purpose: string) => {
    return roleListRequest.data?.map((role: any) => (<Select.Option key={`ROLE_${purpose}_${role.name}`} value={role.name}>{role.name}</Select.Option>)) ?? []
  };

  return (
    <PageContainer>
      <div style={{ marginBottom: "5px", textAlign: "right" }}>
        <Button type="primary" onClick={() => setShowCreate(true)}>创建新用户</Button>
        <Button type="default" onClick={userListRequest.run}>刷新</Button>
      </div>
      <Table dataSource={userListRequest.data} rowKey="id" loading={userListRequest.loading} pagination={false}>
        <Column title="ID" dataIndex="id" />
        <Column title="UserName" dataIndex="userName" />
        <Column title="Roles" dataIndex="rolesStr" />
        <Column title="Action" key="action"
          render={(_: any, user: any) => (
            (user.roles.indexOf('admin') === -1 && <>
              <Button type="link" loading={deleteUserList.fetches[user.id]?.loading} onClick={() => { onDelete(user.id) }}>Delete</Button>
              <Button type="link" loading={getUserByIdRequest.fetches[user.id]?.loading} onClick={() => { onShowEdit(user.id) }}>Edit</Button>
              <Link to={`/users/detail/${user.id}`}>Detail</Link>
            </>)
          )} />
      </Table>

      <Modal title="Create User" visible={showCreate}
        onOk={form.submit}
        onCancel={() => setShowCreate(false)}
        confirmLoading={createUserRequest.loading}
        destroyOnClose>
        <Form form={form} style={{ top: 20 }} {...formLayout}
          onFinish={createUserRequest.run}
          preserve={false}
        >
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
            label="Roles"
            name="roles"
            rules={[{ required: true, type: "array" }]}
          >
            <Select
              mode="multiple"
              size="middle"
              placeholder="Please select role"
              style={{ width: '100%' }}
            >
              {getRoleOptions('create')}
            </Select>
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
            label="Remark"
            name="remark"
            rules={[{ max: 255 }]}
          >
            <Input.TextArea />
          </Form.Item>
        </Form>
      </Modal>

      <Modal title="Edit User" visible={showEdit}
        onOk={editForm.submit}
        onCancel={() => setshowEdit(false)}
        confirmLoading={updateUserRequest.loading}
        destroyOnClose>
        <Form form={editForm} style={{ top: 20 }} {...formLayout}
          onFinish={(model: any) => updateUserRequest.run(model.id, model)}
          preserve={false}
        >
          <Form.Item
            label="Id"
            name="id"
            hidden={true}
            rules={[{ required: true }]}
          >
            <Input />
          </Form.Item>
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
            label="Roles"
            name="roles"
            rules={[{ required: true, type: "array" }]}
          >
            <Select
              mode="multiple"
              size="middle"
              placeholder="Please select role"
              style={{ width: '100%' }}
            >
              {getRoleOptions('edit')}
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