import { PageContainer } from '@ant-design/pro-layout';
import { Button, Table, Modal, message } from 'antd';
import { useRequest } from 'umi';
import { useState } from 'react';
import { secretList, createSecret, deleteSecret } from '@/services/api/secret';
import { toTime } from '@/utils/utils';

const { Column } = Table;

interface SecretModel {
  secretId?: string;
  secretKey?: string;
  createTime?: number;
  userId?: string;
};

export default () => {

  const [secretKeyShowState, setSecretKeyShowState] = useState<any>({});

  const secretListRequest = useRequest(secretList);
  const reload = () => {
    secretListRequest.run();
    message.success('success');
  };
  const createSecretRequest = useRequest(createSecret, {
    manual: true, onSuccess: reload
  });
  const deleteSecretRequest = useRequest(deleteSecret, {
    manual: true, fetchKey: (id: any) => id, onSuccess: reload
  });

  const onDelete = (secretId: string) => {
    Modal.confirm({
      title: 'Are you sure delete this secret key?',
      onOk: async () => {
        deleteSecretRequest.run(secretId);
      }
    });
  };

  const secretKeyContent = (secretKey: string) => {
    return (<span>
      <span>{secretKeyShowState[secretKey] ?? false ? secretKey : "************************"}</span>
      <Button
        type="link"
        onClick={
          () => {

            secretKeyShowState[secretKey] = !(secretKeyShowState[secretKey] ?? false);
            setSecretKeyShowState({ ...secretKeyShowState });
          }
        }>
        {secretKeyShowState[secretKey] ?? false ? "隐藏" : "显示"}
      </Button>
    </span>);
  };

  return (
    <PageContainer>
      <div style={{ marginBottom: "5px", textAlign: "right" }}>
        <Button type="primary" onClick={createSecretRequest.run}>创建密钥</Button>
        <Button type="default" onClick={secretListRequest.run}>刷新</Button>
      </div>
      <Table dataSource={secretListRequest.data} loading={secretListRequest.loading}>
        <Column title="SecretId" dataIndex="secretId" />
        <Column title="Secretkey" dataIndex="secretkey" render={(text: any, item: SecretModel) => secretKeyContent(item.secretKey!)} />
        <Column title="CreateTime" dataIndex="createTime" render={(text: any, item: SecretModel) => <span>{toTime(item.createTime!)}</span>} />
        <Column title="Action" key="action"
          render={(_: any, item: any) => (
            <>
              <Button type="link" loading={deleteSecretRequest.fetches[item.name]?.loading} onClick={() => { onDelete(item.name) }}>Delete</Button>
            </>
          )} />
      </Table>
    </PageContainer >);
};