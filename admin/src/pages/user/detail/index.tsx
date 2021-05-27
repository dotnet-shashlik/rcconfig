import { PageContainer } from '@ant-design/pro-layout';
import { Button, Table, Space, Modal, Form } from 'antd';
import { useRequest } from 'umi';
import { getUserById, userResourceList } from '@/services/api/user';

const { Column } = Table;
const formLayout = {
  labelCol: { span: 3 },
  wrapperCol: { span: 21 },
};

export default (props: any) => {
  const { id } = props.match?.params as any;
  const { data, loading } = useRequest(getUserById, { defaultParams: [id] });
  const userResourceListRequest = useRequest(userResourceList, { defaultParams: id });

  return (<PageContainer loading={loading || userResourceListRequest.loading}>
    <Form {...formLayout}>
      <Form.Item label="ID">
        <span>{data?.id}</span>
      </Form.Item>
      <Form.Item label="User Name">
        <span>{data?.userName}</span>
      </Form.Item>
      <Form.Item label="Nick Name">
        <span>{data?.nickName}</span>
      </Form.Item>
      <Form.Item label="Remark">
        <span>{data?.remark}</span>
      </Form.Item>
      <Form.Item label="Roles">
        <span>{data?.roles}</span>
      </Form.Item>
      <Form.Item label="Resources">
        {
          userResourceListRequest?.data?.map((item: any) => (<li>{item.id} : {item.actionStr}</li>))
        }
      </Form.Item>
    </Form>
  </PageContainer>);
};