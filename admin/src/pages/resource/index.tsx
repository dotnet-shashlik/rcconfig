import { PageContainer } from '@ant-design/pro-layout';
import { Button, Table, Modal, Form, message, Row, Col, Select, Pagination } from 'antd';
import { useRequest } from 'umi';
import { useState, useEffect } from 'react';
import { roleList } from '@/services/api/role';
import { resourceAuthList, resourceList, authRoleResource, unAuthRoleResource } from '@/services/api/resource';

const { Column } = Table;

const formLayout = {
  labelCol: { span: 8 },
  wrapperCol: { span: 16 }
};

interface ResourceModel {
  id?: string;
  resourceType?: string;
  actionStr?: string;
  action?: number;
  actions?: number[];
  role?: string;
}

interface SearchForm {
  role?: string;
  id?: string;
}

interface SearchModel extends SearchForm {
  pageIndex: number;
  pageSize: number;
}

export default (props: any) => {
  const { selectId, selectRole } = props.location.query;
  const [searchModel, setSearchModel] = useState<SearchModel>({ pageIndex: 1, pageSize: 20, id: selectId, role: selectRole });
  const [showCreate, setShowCreate] = useState(false);
  const resourceAuthListRequest = useRequest(resourceAuthList, { defaultParams: [searchModel] });
  const resourceListRequest = useRequest(resourceList);
  const roleListRequest = useRequest(roleList);

  useEffect(() => {
    resourceAuthListRequest.run(searchModel);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [searchModel]);

  const doSearchOfReload = () => {
    resourceAuthListRequest.run(searchModel);
  };
  const doSearchOnPageChange = (pageIndex: number, pageSize?: number) => {
    if (!pageSize)
      setSearchModel({ ...searchModel, pageIndex });
    else
      setSearchModel({ ...searchModel, pageIndex, pageSize });
  };

  const doSearchOnFormSubmit = (model: SearchForm) => {
    setSearchModel({ ...searchModel, ...model, pageIndex: 1 });
  };

  const authRoleResourceRequest = useRequest(authRoleResource, {
    manual: true, onSuccess: () => {
      doSearchOfReload();
      message.success('success');
    }
  });
  const unAuthRoleResourceRequest = useRequest(unAuthRoleResource, {
    manual: true, onSuccess: () => {
      doSearchOfReload();
      message.success('success');
    },
    fetchKey: (data: any) => `${data.resourceId}_${data.role}`
  });

  const onDelete = (model: ResourceModel) => {
    Modal.confirm({
      title: 'Are you sure delete this authorization?',
      onOk: async () => {
        unAuthRoleResourceRequest.run({
          resourceId: model.id!,
          role: model.role
        });
      }
    });
  };

  const [form] = Form.useForm<ResourceModel>();

  const authSubmit = (model: ResourceModel) => {
    let action = model.actions![0];
    // eslint-disable-next-line no-plusplus
    for (let i = 1; i < model.actions!.length; i++) {
      // eslint-disable-next-line no-bitwise
      action |= model.actions![i];
    }

    const data = { resourceId: model.id!, role: model.role, action };
    authRoleResourceRequest.run(data);
  };

  const getRoleOptions = (purpose: string) => {
    return roleListRequest.data?.map((role: any) => (<Select.Option key={`ROLE_${purpose}_${role.name}`} value={role.name}>{role.name}</Select.Option>)) ?? []
  };
  const getResourceOptions = (purpose: string) => {
    return resourceListRequest.data?.map((resource: any) => (<Select.Option key={`RESOURCE_${purpose}_${resource.id}`} value={resource.id}>{resource.id}</Select.Option>)) ?? []
  };
  const getActionOptions = (purpose: string) => {
    return (
      <>
        <Select.Option key={`action1_${purpose}`} value={1}>READ</Select.Option>
        <Select.Option key={`action2_${purpose}`} value={2}>WRITE</Select.Option>
        <Select.Option key={`action4_${purpose}`} value={4}>DELETE</Select.Option>
      </>
    )
  };

  return (
    <PageContainer>
      <Row style={{ marginBottom: "5px" }}>
        <Col span={16}>
          <Form
            layout="inline"
            {...formLayout}
            initialValues={searchModel}
            onFinish={(model: SearchForm) => doSearchOnFormSubmit(model)}
          >
            <Form.Item
              label="资源Id"
              name="id"
            >
              <Select
                size="middle"
                placeholder="Please select resource"
                allowClear
                style={{ width: '260px' }}
              >
                {getResourceOptions('search')}
              </Select>
            </Form.Item>
            <Form.Item
              label="角色"
              name="role"
            >
              <Select
                size="middle"
                placeholder="Please select resource"
                allowClear
                style={{ width: '260px' }}
              >
                {getRoleOptions('search')}
              </Select>
            </Form.Item>
            <Form.Item>
              <Button type="primary" htmlType="submit" >查询</Button>
            </Form.Item>
          </Form>
        </Col>
        <Col span={8} style={{ textAlign: "right" }}>
          <Button type="primary" onClick={() => setShowCreate(true)}>资源授权</Button>
          <Button type="default" onClick={doSearchOfReload}>刷新</Button>
        </Col>
      </Row>
      <Table dataSource={resourceAuthListRequest.data?.rows} loading={resourceAuthListRequest.loading}
        pagination={{
          pageSize: searchModel.pageSize,
          defaultCurrent: searchModel.pageIndex,
          total: resourceAuthListRequest.data?.total ?? 0,
          onChange: doSearchOnPageChange
        }}>
        <Column title="Resource Id" dataIndex="id" />
        <Column title="Resource Type" dataIndex="resourceType" />
        <Column title="Authorization Role" dataIndex="role" />
        <Column title="Action" dataIndex="actionStr" />
        <Column title="Action" key="action"
          render={(_: any, item: any) => (
            <span>
              <Button type="link" loading={unAuthRoleResourceRequest.fetches[`${item.id}_${item.role}`]?.loading} onClick={() => { onDelete(item) }}>Delete</Button>
            </span>
          )} />
      </Table>
      <Modal title="Create Role" visible={showCreate}
        onOk={form.submit}
        onCancel={() => setShowCreate(false)}
        confirmLoading={authRoleResourceRequest.loading}
        destroyOnClose
      >
        <Form form={form} initialValues={searchModel} style={{ top: 20 }} {...formLayout}
          onFinish={authSubmit}
          preserve={false}
        >
          <Form.Item
            label="资源Id"
            name="id"
            rules={[{ required: true }]}
          >
            <Select
              size="middle"
              placeholder="Please select resource"
              style={{ width: '100%' }}
            >
              {getResourceOptions('auth')}
            </Select>
          </Form.Item>
          <Form.Item
            label="Role Name"
            name="role"
            rules={[{ required: true }]}
          >
            <Select
              size="middle"
              placeholder="Please select role"
              style={{ width: '100%' }}
            >
              {getRoleOptions('auth')}
            </Select>
          </Form.Item>
          <Form.Item
            label="Select Action"
            name="actions"
            rules={[{ required: true, min: 1, max: 3, type: 'array' }]}
          >
            <Select
              mode="multiple"
              size="middle"
              placeholder="Please select action"
              style={{ width: '100%' }}
            >
              {getActionOptions('auth')}
            </Select>
          </Form.Item>
        </Form>
      </Modal>
    </PageContainer>);
};