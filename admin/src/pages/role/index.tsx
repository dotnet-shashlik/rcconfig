import { PageContainer } from '@ant-design/pro-layout';
import { Button, Table, Modal, Form, Input, message } from 'antd';
import { Link, useRequest } from 'umi';
import { useState } from 'react';
import { roleList, createRole, deleteRole, resourceList } from '@/services/api/role';

const { Column } = Table;

const formLayout = {
  labelCol: { span: 8 },
  wrapperCol: { span: 13 },
};

export default () => {

  const [showCreate, setShowCreate] = useState(false);
  const roleListRequest = useRequest(roleList);
  const reload = () => {
    roleListRequest.run();
    setShowCreate(false);
    message.success('success');
  };
  const createRoleRequest = useRequest(createRole, {
    manual: true, onSuccess: reload
  });
  const deleteRoleList = useRequest(deleteRole, {
    manual: true, fetchKey: (role: string) => role, onSuccess: reload
  });

  const onDelete = (role: string) => {
    Modal.confirm({
      title: 'Are you sure delete this role?',
      onOk: async () => {
        deleteRoleList.run(role);
      }
    });
  };
  const [form] = Form.useForm();

  const onSubmit = () => {
    form.validateFields()
      .then(formValues => {
        createRoleRequest.run(formValues);
      })
  }

  return (
    <PageContainer>
      <div style={{ marginBottom: "5px", textAlign: "right" }}>
        <Button type="primary" onClick={() => setShowCreate(true)}>创建新角色</Button>
        <Button type="default" onClick={roleListRequest.run}>刷新</Button>
      </div>
      <Table dataSource={roleListRequest.data} loading={roleListRequest.loading}>
        <Column title="ID" dataIndex="id" />
        <Column title="Name" dataIndex="name" />
        <Column title="Action" key="action"
          render={(_: any, item: any) => (
            <span>
              <Button type="link" loading={deleteRoleList.fetches[item.name]?.loading} onClick={() => { onDelete(item.name) }}>Delete</Button>
              <Link to={`/resources?selectRole=${item.name}`}>Resources</Link>
            </span>
          )} />
      </Table>

      <Modal title="Create Role" visible={showCreate} onOk={onSubmit} onCancel={() => setShowCreate(false)}
        confirmLoading={createRoleRequest.loading}>
        <Form form={form} style={{ top: 20 }} {...formLayout}>
          <Form.Item
            label="Role Name"
            name="name"
            rules={[{ required: true }, { max: 32 }]}
          >
            <Input />
          </Form.Item>
        </Form>
      </Modal>
    </PageContainer>);
};