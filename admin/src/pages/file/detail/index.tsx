/* eslint-disable react-hooks/exhaustive-deps */
import { PageContainer } from '@ant-design/pro-layout';
import { Button, Form, Input, message, Radio } from 'antd';
import { useRequest, history } from 'umi';
import { useState, useEffect } from 'react';
import { createFile, updateFile, fileDetail } from '@/services/api/file';

const CodeMirror = require('codemirror/lib/codemirror.js');
require('codemirror/lib/codemirror.css');
require('codemirror/theme/material-darker.css');
require('codemirror/mode/xml/xml');
require('codemirror/mode/yaml/yaml');
require('codemirror/mode/properties/properties');
require('codemirror/mode/javascript/javascript');
require('codemirror/addon/hint/show-hint.css');
require('codemirror/addon/hint/show-hint.js');

interface FormModel {
  id?: number;
  name?: string;
  desc?: string;
  type?: string;
  content?: string;
}

const formLayout = {
  labelCol: { span: 2 },
  wrapperCol: { span: 10 },
};
const tailLayout = {
  wrapperCol: { offset: 2, span: 20 },
};

export default (props: any) => {
  const { app, env } = props.match.params as any;
  const { fileId } = props.location.query;
  if (!app || !env) {
    history.push('/404');
  }
  const resourceId = `${app}/${env}`;
  const [form] = Form.useForm<FormModel>();

  const createFileRequest = useRequest(createFile, {
    manual: true, onSuccess: () => {
      history.push(`/files?selectResourceId=${resourceId}`);
      message.success(`success`);
    }
  });
  const updateFileRequest = useRequest(updateFile, {
    manual: true,
    onSuccess: () => message.success(`success`)
  });

  const [editor, setEditor] = useState<any>(false);
  useEffect(() => {
    const e = CodeMirror.fromTextArea(document.getElementById("content"), {
      lineNumbers: true,
      mode: 'yaml',
      matchBrackets: true,
      theme: 'material-darker',
      indentWithTabs: false,
      tabSize: 4,
      smartIndent: true,
      indentUnit: 4
    });
    e.setSize("100%", "350px");
    e.on('change', () => {
      form.setFieldsValue({ ...form.getFieldsValue(), content: e.getValue() });
    });

    setEditor(e);
  }, [true]);
  if (fileId && fileId > 0) {
    useRequest(fileDetail, {
      defaultParams: [resourceId, fileId], onSuccess: (data: FormModel) => {
        form.setFieldsValue(data);
        editor.setValue(data.content);
      }
    });
  }

  const onTypeChange = (e: any) => {
    let type = e.target.value;
    if (editor) {
      if (type === 'json') {
        type = 'javascript';
      }
      editor.setOption('mode', type);
    }
  };

  const onSubmit = (model: FormModel) => {
    // eslint-disable-next-line no-param-reassign
    model.content = model.content?.replaceAll('\t', '    ');
    if (!model.id)
      createFileRequest.run(resourceId, model);
    else
      updateFileRequest.run(resourceId, model.id, model);
  }

  return (<PageContainer>
    <h1>{app}/{env} 创建文件</h1>
    <Form
      {...formLayout}
      form={form}
      style={{ top: 20 }}
      layout="horizontal"
      initialValues={{ type: 'yaml' }}
      onFinish={onSubmit}
    >
      <Form.Item
        label="id"
        name="id"
        hidden
      >
        <Input />
      </Form.Item>
      <Form.Item
        label="File Name"
        name="name"
        rules={[{ required: true, max: 32 }]}
      >
        <Input />
      </Form.Item>
      <Form.Item
        label="Description"
        name="desc"
        rules={[{ max: 255 }]}
      >
        <Input />
      </Form.Item>
      <Form.Item
        label="Type"
        name="type"
        rules={[{ required: true, max: 255 }]}
      >
        <Radio.Group onChange={onTypeChange}>
          <Radio value="yaml">yaml</Radio>
          <Radio value="json">json</Radio>
          <Radio value="xml">xml</Radio>
          <Radio value="properties">properties</Radio>
        </Radio.Group>
      </Form.Item>
      <Form.Item
        label="Content"
        name="content"
        rules={[{ required: true }]}
      >
        <Input.TextArea></Input.TextArea>
      </Form.Item>
      <Form.Item {...tailLayout}>
        <Button type="primary" htmlType="submit" loading={createFileRequest.loading || updateFileRequest.loading} >Submit</Button>&nbsp;&nbsp;
        <Button type="default" onClick={() => history.push(`/files?selectResourceId=${resourceId}`)} loading={createFileRequest.loading || updateFileRequest.loading} >Back</Button>
      </Form.Item>
    </Form>
  </PageContainer>);
};