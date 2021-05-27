import { PageContainer } from '@ant-design/pro-layout';
import { Button, Table, Modal, Form, Input, message, Row, Col, Space, Divider, Select } from 'antd';
import { Link, useRequest, history } from 'umi';
import { useState, useEffect } from 'react';
import { envList, createEnv, deleteEnv, updateEnv } from '@/services/api/env';
import { Regexs, toTime } from '@/utils/utils';
import { resourceList } from '@/services/api/resource';

const { Column } = Table;

const formLayout = {
  labelCol: { span: 8 },
  wrapperCol: { span: 13 },
};

interface EnvModel {
  name?: string;
  desc?: string;
  ipWhites?: string;
  applicationId?: number;
  applicationName?: string;
  createTime?: number;
  resourceId?: string;
};

interface ResouceModel {
  id: string;
  resourceType: string;
}

export default (props: any) => {
  const { app } = props.location.query;
  const [showCreate, setShowCreate] = useState(false);
  const [showEdit, setShowEdit] = useState(false);
  const envListRequest = useRequest(envList, { manual: true });

  useEffect(() => {
    if (!app) {
      return;
    }
    envListRequest.run(app);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [props]);

  const goSearch = (goApp: string) => {
    history.replace(`/envs?app=${goApp}`);
  }

  const doRefresh = () => {
    if (app) {
      history.replace(`/envs?app=${app}`);
    }
  }

  const reload = () => {
    setShowCreate(false);
    setShowEdit(false);
    message.success('success');
    doRefresh();
  };
  const createEnvRequest = useRequest(createEnv, {
    manual: true, onSuccess: reload
  });
  const updateEnvRequest = useRequest(updateEnv, {
    manual: true, onSuccess: reload
  });
  const deleteEnvRequest = useRequest(deleteEnv, {
    manual: true, fetchKey: (appP: string, envP: string) => `${appP}/${envP}`, onSuccess: reload
  });
  const [resources, setResources] = useState<ResouceModel[]>([]);
  useRequest(resourceList, {
    onSuccess: (data: ResouceModel[]) => {
      setResources(data.filter(r => r.resourceType === 'Application'));
    }
  });
  const getResourceOptions = (purpose: string) => {
    return resources.map((resource: any) => (<Select.Option key={`RESOURCE_${purpose}_${resource.id}`} value={resource.id}>{resource.id}</Select.Option>)) ?? []
  };

  const onDelete = (env: string) => {
    Modal.confirm({
      title: 'Are you sure delete this environment?',
      onOk: async () => {
        deleteEnvRequest.run(app, env);
      }
    });
  };

  const [form] = Form.useForm();
  const [editForm] = Form.useForm();

  const onShowEdit = (envModel: EnvModel) => {
    editForm.setFieldsValue(envModel);
    setShowEdit(true);
  };

  return (
    <PageContainer>
      <Row style={{ marginBottom: "5px" }}>
        <Col span={16}>
          <Select
            size="middle"
            placeholder="Please select application"
            style={{ width: '260px' }}
            onSelect={goSearch}
            defaultValue={app}
          >
            {getResourceOptions('search')}
          </Select>
        </Col>
        <Col span={8} style={{ textAlign: "right" }}>
          {app && (
            <>
              <Button type="primary" onClick={() => setShowCreate(true)}>创建环境</Button>
              <Button type="default" onClick={() => doRefresh()}>刷新</Button>
            </>
          )}
        </Col>
      </Row>
      <Table dataSource={envListRequest.data} rowKey="id" loading={envListRequest.loading} pagination={false}>
        <Column title="Name" dataIndex="name" />
        <Column title="ResourceId" dataIndex="resourceId" />
        <Column title="Description" dataIndex="desc" />
        <Column title="CreateTime" dataIndex="createTime" render={(text: any, item: EnvModel) => <span>{toTime(item.createTime!)}</span>} />
        <Column title="Action" key="action"
          render={(_: any, item: any) => (
            <Space>
              <Button type="link" loading={deleteEnvRequest.fetches[item.resourceId]?.loading} onClick={() => { onDelete(item.name) }}>Delete</Button>
              <Button type="link" onClick={() => { onShowEdit(item) }}>Edit</Button>
              <Link to={`/files?selectResourceId=${item.resourceId}`}>Files</Link>
              <Link to={`/files/detail/${item.resourceId}`}>Create File</Link>
              <Link to={`/resources?selectId=${item.resourceId}`}>Authorization</Link>
            </Space>
          )} />
      </Table>

      <Modal
        title="Create Environment"
        visible={showCreate}
        onOk={form.submit}
        onCancel={() => setShowCreate(false)}
        confirmLoading={updateEnvRequest.loading}
        destroyOnClose
      >
        <Form form={form} style={{ top: 20 }} {...formLayout}
          onFinish={(model) => createEnvRequest.run(app, model)}
          preserve={false}>
          <Form.Item
            label="Environment Name"
            name="name"
            rules={[{ required: true, max: 32, pattern: Regexs.resourceName }]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            label="Description"
            name="desc"
            rules={[{ required: true, max: 255 }]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            label="IpWhites"
            name="ipWhites"
            rules={[{ max: 255 }]}
          >
            <Input.TextArea />
          </Form.Item>
        </Form>
      </Modal>

      <Modal
        title="Edit Environment"
        visible={showEdit}
        onOk={editForm.submit}
        onCancel={() => setShowCreate(false)}
        confirmLoading={createEnvRequest.loading}
        destroyOnClose
      >
        <Form form={editForm} style={{ top: 20 }} {...formLayout}
          onFinish={(model) => updateEnvRequest.run(app, model.name!, model)}
          preserve={false}
        >
          <Form.Item
            label="Environment Name"
            name="name"
            rules={[{ required: true, max: 32, pattern: Regexs.resourceName }]}
          >
            <Input disabled />
          </Form.Item>
          <Form.Item
            label="Description"
            name="desc"
            rules={[{ required: true, max: 255 }]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            label="IpWhites"
            name="ipWhites"
            rules={[{ max: 255 }]}
          >
            <Input.TextArea />
          </Form.Item>
        </Form>
      </Modal>
    </PageContainer >);
};