import { PageContainer } from '@ant-design/pro-layout';
import { Button, Table, Modal, Form, Input, message, Row, Col } from 'antd';
import { Link, useRequest } from 'umi';
import { useState } from 'react';
import { roleList } from '@/services/api/role';
import { resourceAuthList, resourceList, authRoleResource, unAuthRoleResource } from '@/services/api/resource';

const { Column } = Table;

const formLayout = {
  labelCol: { span: 8 },
  wrapperCol: { span: 13 },
};

interface ResourceModel {
  id?: string;
  resourceType?: string;
  cctionStr?: string;
  action?: number;
  role?: string;
}

interface SearchModel {
  pageIndex: number;
  pageSize: number;
  role?: string;
  id?: string;
}

export default (props: any) => {
  const { id } = props.location.query;
  const [searchModel, setSearchModel] = useState<SearchModel>({ pageIndex: 1, pageSize: 2 });
  const [showCreate, setShowCreate] = useState(false);
  const resourceAuthListRequest = useRequest(resourceAuthList, { defaultParams: [searchModel] });
  const resourceListRequest = useRequest(resourceList);
  const roleListRequest = useRequest(roleList);
  const reload = (newSearchModel: SearchModel) => {
    setSearchModel(newSearchModel);
    resourceAuthListRequest.run(searchModel);
    setShowCreate(false);
    message.success('success');
  };

  const authRoleResourceRequest = useRequest(authRoleResource, {
    manual: true, onSuccess: reload
  });
  const unAuthRoleResourceRequest = useRequest(unAuthRoleResource, {
    manual: true, onSuccess: reload
  });

  const onDelete = (item: ResourceModel) => {
    Modal.confirm({
      title: 'Are you sure delete this role?',
      onOk: async () => {
        unAuthRoleResourceRequest.run(item.id!, {
          role: item.role
        });
      }
    });
  };
  const [form] = Form.useForm();

  const getRoleOptions = () => {
    return roleListRequest.data?.map((role: string) => {
      return (<Select.Option key={`ROLE_${role}`} value={role}>{role}</Select.Option>);
    }
  };

  return (
    <PageContainer>
      <Row style={{ marginBottom: "5px" }}>
        <Col span={12}>
          <Form form={form} style={{ top: 20 }} {...formLayout}
          >
            <Form.Item
              label="资源Id"
              name="name"
              rules={[{ required: true }, { max: 32 }]}
            >
              <Input />
            </Form.Item>
            <Form.Item
              label="角色"
              name="name"
              rules={[{ required: true }, { max: 32 }]}
            >
              <Input />
            </Form.Item>
            <Form.Item>
              <Button type="primary" htmlType="submit" value="查询" />
            </Form.Item>
          </Form>
        </Col>
        <Col span={12} style={{ textAlign: "right" }}>
          <Button type="primary" onClick={() => setShowCreate(true)}>资源授权</Button>
          <Button type="default" onClick={() => reload(searchModel)}>刷新</Button>
        </Col>
      </Row>
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