import { PageContainer } from '@ant-design/pro-layout';
import { Button, Table, Modal, message, Row, Col, Select } from 'antd';
import { useRequest, history, Link } from 'umi';
import { useState, useEffect } from 'react';
import { fileList, deleteFile } from '@/services/api/file';
import { resourceList } from '@/services/api/resource';

const { Column } = Table;

interface SearchModel {
  pageIndex: number;
  pageSize: number;
}

interface ResouceModel {
  id: string;
  resourceType: string;
}

export default (props: any) => {

  const { selectResourceId } = props.location.query;
  const [searchModel, setSearchModel] = useState<SearchModel>({ pageIndex: 1, pageSize: 20 });
  const fileListRequest = useRequest(fileList, { manual: true });
  const [resources, setResources] = useState<ResouceModel[]>([]);
  useRequest(resourceList, {
    onSuccess: (data: ResouceModel[]) => {
      setResources(data.filter(r => r.resourceType === 'Environment'));
    }
  });

  useEffect(() => {
    if (!selectResourceId) {
      return;
    }
    fileListRequest.run(selectResourceId, searchModel);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [props, searchModel]);

  const goSearch = (goResource: string) => {
    history.replace(`/files?selectResourceId=${goResource}`);
  };
  const doRefresh = () => {
    if (selectResourceId) {
      setSearchModel({ ...searchModel });
    }
  };

  const doSearchOfReload = () => {
    setSearchModel({ ...searchModel });
  };
  const doSearchOnPageChange = (pageIndex: number, pageSize?: number) => {
    if (!pageSize)
      setSearchModel({ ...searchModel, pageIndex });
    else
      setSearchModel({ ...searchModel, pageIndex, pageSize });
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
          <Select
            size="middle"
            placeholder="Please select resource"
            style={{ width: '260px' }}
            onSelect={goSearch}
            defaultValue={selectResourceId}
          >
            {getResourceOptions('search')}
          </Select>
        </Col>
        <Col span={8} style={{ textAlign: "right" }}>
          {selectResourceId && (
            <>
              <Button type="primary" onClick={() => history.push(`/files/detail/${selectResourceId}`)}>New File</Button>
              <Button type="default" onClick={doRefresh}>刷新</Button>
            </>
          )}
        </Col>
      </Row>
      <Table dataSource={fileListRequest.data?.rows ?? []} rowKey="id" loading={fileListRequest.loading}
        pagination={{
          pageSize: searchModel.pageSize,
          defaultCurrent: searchModel.pageIndex,
          total: fileListRequest.data?.total ?? 0,
          onChange: doSearchOnPageChange
        }}>
        <Column title="ID" dataIndex="id" />
        <Column title="Name" dataIndex="name" render={(_, item: any) => (<Link to={`/files/detail/${selectResourceId}?fileId=${item.id}`}>{item.name}</Link>)} />
        <Column title="File Type" dataIndex="type" />
        <Column title="Description" dataIndex="desc" />
        <Column title="Action" key="action"
          render={(_: any, item: any) => (
            <span>
              <Button type="link" loading={deleteFileRequest.fetches[`${item.environmentResourceId}/${item.id}`]?.loading} onClick={() => { onDelete(item) }}>Delete</Button>
              <Button type="link" onClick={() => { history.push(`/logsbyfileid/${selectResourceId}/${item.id}`) }}>Change Logs</Button>
            </span>
          )} />
      </Table>
    </PageContainer>);
};