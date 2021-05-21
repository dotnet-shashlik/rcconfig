import { PageContainer } from '@ant-design/pro-layout';
import { Button, Table, Modal, Form, Input, message } from 'antd';
import { Link, useRequest } from 'umi';
import { useState } from 'react';
import { appList, createApp, deleteApp, updateApp } from '@/services/api/app';
import { Regexs, toTime } from '@/utils/utils';

const { Column } = Table;

const formLayout = {
  labelCol: { span: 8 },
  wrapperCol: { span: 13 },
};

interface AppModel {
  name?: string;
  desc?: string;
  createTime?: number;
  resourceId?: string;
};

export default () => {

  const [showCreate, setShowCreate] = useState(false);
  const [showEdit, setShowEdit] = useState(false);
  const appListRequest = useRequest(appList);
  const reload = () => {
    appListRequest.run();
    setShowCreate(false);
    setShowEdit(false);
    message.success('success');
  };
  const createAppRequest = useRequest(createApp, {
    manual: true, onSuccess: reload
  });
  const updateAppRequest = useRequest(updateApp, {
    manual: true, onSuccess: reload
  });
  const deleteAppRequest = useRequest(deleteApp, {
    manual: true, fetchKey: (app: string) => app, onSuccess: reload
  });

  const onDelete = (app: string) => {
    Modal.confirm({
      title: 'Are you sure delete this application?',
      onOk: async () => {
        deleteAppRequest.run(app);
      }
    });
  };

  const [form] = Form.useForm<AppModel>();
  const [editForm] = Form.useForm<AppModel>();

  const onShowEdit = (appModel: AppModel) => {
    editForm.setFieldsValue(appModel);
    setShowEdit(true);
  };

  return (
    <PageContainer>
      <div style={{ marginBottom: "5px", textAlign: "right" }}>
        <Button type="primary" onClick={() => setShowCreate(true)}>创建应用</Button>
        <Button type="default" onClick={appListRequest.run}>刷新</Button>
      </div>
      <Table dataSource={appListRequest.data} loading={appListRequest.loading} pagination={false}>
        <Column title="Name" dataIndex="name" render={(_: any, item: AppModel) => <Link to={`/envs/${item.name}`}>{item.name}</Link>} />
        <Column title="ResourceId" dataIndex="resourceId" />
        <Column title="Description" dataIndex="desc" />
        <Column title="CreateTime" dataIndex="createTime" render={(_: any, item: AppModel) => <span>{toTime(item.createTime!)}</span>} />
        <Column title="Action" key="action"
          render={(_: any, item: any) => (
            <span>
              <Button type="link" loading={deleteAppRequest.fetches[item.name]?.loading} onClick={() => { onDelete(item.name) }}>Delete</Button>
              <Button type="link" onClick={() => { onShowEdit(item) }}>Edit</Button>
              <Link to={`/envs/${item.name}`}>{item.name}</Link>
              <Link to={`/resources?selectId=${item.resourceId}`}>Authorization</Link>
            </span>
          )} />
      </Table>

      <Modal
        title="Create Application"
        visible={showCreate}
        onOk={form.submit}
        onCancel={() => setShowCreate(false)}
        confirmLoading={createAppRequest.loading}
        destroyOnClose
      >
        <Form form={form} style={{ top: 20 }} {...formLayout}
          onFinish={createAppRequest.run} preserve={false}>
          <Form.Item
            label="Application Name"
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
        </Form>
      </Modal>

      <Modal
        title="Edit Application"
        visible={showEdit}
        onOk={editForm.submit}
        onCancel={() => setShowEdit(false)}
        confirmLoading={updateAppRequest.loading}
        destroyOnClose
      >
        <Form form={editForm} style={{ top: 20 }} {...formLayout}
          onFinish={createAppRequest.run} preserve={false}
        >
          <Form.Item
            label="Application Name"
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
          >
            <Button type="primary" htmlType="submit" />
          </Form.Item>
        </Form>
      </Modal>
    </PageContainer >);
};