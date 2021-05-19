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
  const [showResources, setShowResources] = useState(false);
  const roleListRequest = useRequest(roleList);
  const reload = () => {
    roleListRequest.run();
    setShowCreate(false);
    message.success('success');
  };
  const createUserRequest = useRequest(createRole, {
    manual: true, onSuccess: reload
  });
  const deleteUserList = useRequest(deleteRole, {
    manual: true, fetchKey: (id: any) => id, onSuccess: reload
  });
  const resourceListRequest = useRequest(resourceList, {
    manual: true, onSuccess: () => {
      setShowResources(true);
    }
  });

  const onDelete = (role: string) => {
    Modal.confirm({
      title: 'Are you sure delete this role?',
      onOk: async () => {
        deleteUserList.run(role);
      }
    });
  };
  const [form] = Form.useForm();

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
          render={(text: any, item: any) => (
            <span>
              <Button type="link" loading={deleteUserList.fetches[item.name]?.loading} onClick={() => { onDelete(item.name) }}>Delete</Button>
              <Button type="link" loading={deleteUserList.fetches[item.name]?.loading} onClick={() => { resourceListRequest.run(item.name) }}>Resources</Button>
            </span>
          )} />
      </Table>

      <Modal title="Create Role" visible={showCreate} onOk={() => createUserRequest.run(form.getFieldsValue())} onCancel={() => setShowCreate(false)}>
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
      <Modal title="Resources" visible={showResources} onCancel={() => setShowResources(false)}>
        <ul>
          <li>
            <Link to="/resources">+ Bind Resource</Link>
          </li>
          {
            resourceListRequest?.data?.map((item: any) => <li>{item.id}:{item.actionStr}</li>)
          }
        </ul>
      </Modal>
    </PageContainer>);
};