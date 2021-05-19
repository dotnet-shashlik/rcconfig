// @ts-ignore
/* eslint-disable */
import { request } from 'umi';

/** 获取密钥列表 GET /secrets */
export async function secretList(options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/secrets`, {
    method: 'GET',
    ...(options || {})
  });
}

/** 创建密钥 POST /secrets */
export async function createSecret(options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/secrets`, {
    method: 'POST',
    ...(options || {})
  });
}

/** 删除密钥 DELETE /secrets */
export async function deleteSecret(secretId: string, options?: { [key: string]: any }) {
  return await request<API.Response<any>>(`/secrets/${secretId}`, {
    method: 'DELETE',
    ...(options || {})
  });
}
