import { PageContainer } from '@ant-design/pro-layout';
import { Button, Table, Space, Modal } from 'antd';
import { Link, useRequest } from 'umi';
import { userList, deleteUser } from '@/services/api/user';

const { Column } = Table;

export default () => {

  const userListRequest = useRequest(userList);
  const deleteUserList = useRequest(deleteUser, {
    manual: true, fetchKey: (obj: any) => {
      return obj.id;
    }
  });

  const onDelete = async (userId: number) => {
    Modal.confirm({
      title: 'Confirm delete this user?',
      onOk: async () => {
        deleteUserList.run(userId);
      }
    });
  };

  return (
    <PageContainer>
      <Table dataSource={userListRequest.data} loading={userListRequest.loading}>
        <Column title="ID" dataIndex="id" />
        <Column title="UserName" dataIndex="userName" />
        <Column title="Action" key="action"
          render={(text: any, user: any) => (
            <Space size="middle">
              <Button type="link" loading={deleteUserList.fetches[user.id]?.loading} onClick={() => { onDelete(user.id) }}>Delete</Button>
              <Link to={`/user/detail/${user.id}`} >Detail</Link>
            </Space>
          )} />
      </Table>
    </PageContainer>);
};