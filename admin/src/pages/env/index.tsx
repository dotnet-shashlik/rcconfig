import { PageContainer } from '@ant-design/pro-layout';
import { Button, Table, Modal, Form, Input, message, Row, Col, Space, Divider } from 'antd';
import { Link, useRequest } from 'umi';
import { useState } from 'react';
import { envList, createEnv, deleteEnv, updateEnv } from '@/services/api/env';
import { Regexs, toTime } from '@/utils/utils';

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

export default (props: any) => {
  const { app } = props.match?.params as any;
  const [showCreate, setShowCreate] = useState(false);
  const [showEdit, setShowEdit] = useState(false);
  const envListRequest = useRequest(envList, { defaultParams: [app] });
  const reload = () => {
    envListRequest.run(app);
    setShowCreate(false);
    message.success('success');
  };
  const createEnvRequest = useRequest(createEnv, {
    manual: true, onSuccess: reload
  });
  const updateEnvRequest = useRequest(updateEnv, {
    manual: true, onSuccess: reload
  });
  const deleteEnvRequest = useRequest(deleteEnv, {
    manual: true, fetchKey: (id: any) => id, onSuccess: reload
  });

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

  const onEdit = (envModel: EnvModel) => {
    editForm.setFieldsValue(envModel);
    setShowEdit(true);
  };

  return (
    <PageContainer>

      <Row style={{ marginBottom: "5px" }}>
        <Col span={12}>
          <h3>Application: {app}</h3>
        </Col>
        <Col span={12} style={{ textAlign: "right" }}>
          <Button type="primary" onClick={() => setShowCreate(true)}>创建环境</Button>
          <Button type="default" onClick={() => envListRequest.run(app)}>刷新</Button>
        </Col>
      </Row>
      <Table dataSource={envListRequest.data} loading={envListRequest.loading}>
        <Column title="Name" dataIndex="name" />
        <Column title="ResourceId" dataIndex="resourceId" />
        <Column title="Description" dataIndex="desc" />
        <Column title="CreateTime" dataIndex="createTime" render={(text: any, item: EnvModel) => <span>{toTime(item.createTime!)}</span>} />
        <Column title="Action" key="action"
          render={(_: any, item: any) => (
            <span>
              <Button type="link" loading={deleteEnvRequest.fetches[item.name]?.loading} onClick={() => { onDelete(item.name) }}>Delete</Button>
              <Button type="link" onClick={() => { onEdit(item) }}>Edit</Button>
              <Link to={`/resources?id=${item.name}`}>Authorization</Link>
            </span>
          )} />
      </Table>

      <Modal
        title="Create Environment"
        visible={showCreate}
        onOk={() => createEnvRequest.run(app, form.getFieldsValue())}
        onCancel={() => setShowCreate(false)}
      >
        <Form form={form} style={{ top: 20 }} {...formLayout}>
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
        onOk={() => updateEnvRequest.run(app, form.getFieldValue('name'), form.getFieldsValue())}
        onCancel={() => setShowCreate(false)}
      >
        <Form form={editForm} style={{ top: 20 }} {...formLayout}>
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