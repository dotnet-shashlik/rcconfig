import { PageContainer } from '@ant-design/pro-layout';
import { Button, Table, Modal, Form, Input, message, Row, Col, Select } from 'antd';
import { Link, useRequest } from 'umi';
import { useState, useEffect } from 'react';
import { fileList, createFile, deleteFile, updateFile } from '@/services/api/file';
import { resourceList } from '@/services/api/resource';

const { Column } = Table;

const formLayout = {
  labelCol: { span: 8 },
  wrapperCol: { span: 13 },
};

interface SearchModel {
  pageIndex: number;
  pageSize: number;
  resourceId: string;
}

interface ResouceModel {
  id: string;
  resourceType: string;
}

export default () => {

  const [searchModel, setSearchModel] = useState<SearchModel>({ pageIndex: 1, pageSize: 20, resourceId: '' });
  const [resources, setResources] = useState<ResouceModel[]>([]);
  const fileListRequest = useRequest(fileList, { manual: true });
  useRequest(resourceList, {
    onSuccess: (data: ResouceModel[]) => {
      setResources(data.filter(r => r.resourceType === 'Environment'));
    }
  });

  useEffect(() => {
    if (!searchModel.resourceId) {
      message.error('请选择资源');
      return;
    }
    fileListRequest.run(searchModel.resourceId, searchModel);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [searchModel]);

  const doSearchOfReload = () => {
    setSearchModel({ ...searchModel });
  };
  const doSearchOnPageChange = (pageIndex: number, pageSize?: number) => {
    if (!pageSize)
      setSearchModel({ ...searchModel, pageIndex });
    else
      setSearchModel({ ...searchModel, pageIndex, pageSize });
  };
  const doSearchOnFormSubmit = (model: SearchModel) => {
    setSearchModel({ ...searchModel, ...model, pageIndex: 1 });
  };

  const deleteFileRequest = useRequest(deleteFile, {
    manual: true, fetchKey: (resourceId: string, fileId: number) => `${resourceId}/${fileId}`, onSuccess: () => {
      doSearchOfReload();
      message.success('success')
    }
  });

  const onDelete = (file: any) => {
    Modal.confirm({
      title: 'Are you sure delete this file?',
      onOk: async () => {
        deleteFileRequest.run(file.environmentResourceId, file.id);
      }
    });
  };
  const getResourceOptions = (purpose: string) => {
    return resources.map((resource: any) => (<Select.Option key={`RESOURCE_${purpose}_${resource.id}`} value={resource.id}>{resource.id}</Select.Option>)) ?? []
  };
  return (
    <PageContainer>
      <Row style={{ marginBottom: "5px" }}>
        <Col span={16}>
          <Form
            layout="inline"
            {...formLayout}
            initialValues={searchModel}
            onFinish={(model: SearchModel) => doSearchOnFormSubmit(model)}
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
            <Form.Item>
              <Button type="primary" htmlType="submit" >查询</Button>
            </Form.Item>
          </Form>
        </Col>
        <Col span={8} style={{ textAlign: "right" }}>
          {/* <Button type="primary" onClick={() => setShowCreate(true)}>创建文件</Button> */}
          <Button type="default" onClick={doSearchOfReload}>刷新</Button>
        </Col>
      </Row>
      <Table dataSource={fileListRequest.data} rowKey="id" loading={fileListRequest.loading}>
        <Column title="Name" dataIndex="environmentResourceId" />
        <Column title="ID" dataIndex="id" />
        <Column title="Name" dataIndex="name" />
        <Column title="Name" dataIndex="type" />
        <Column title="Name" dataIndex="desc" />
        <Column title="Action" key="action"
          render={(_: any, item: any) => (
            <span>
              <Button type="link" loading={deleteFileRequest.fetches[`${item.environmentResourceId}/${item.id}`]?.loading} onClick={() => { onDelete(item) }}>Delete</Button>
              {/* <Button type="link" loading={deleteFileList.fetches[item.name]?.loading} onClick={() => { fileListRequest.run(item.name) }}>修改记录</Button> */}
            </span>
          )} />
      </Table>
    </PageContainer>);
};