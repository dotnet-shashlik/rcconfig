export default [
  {
    path: '/user',
    layout: false,
    routes: [
      {
        path: '/user',
        routes: [
          {
            name: 'login',
            path: '/user/login',
            component: './user/Login',
          },
        ],
      },
    ],
  },
  {
    path: '/welcome',
    name: 'welcome',
    icon: 'smile',
    component: './Welcome',
  },
  {
    path: '/admin',
    name: 'admin',
    icon: 'crown',
    component: './Admin',
    routes: [
      {
        path: '/admin/sub-page',
        name: 'sub-page',
        icon: 'smile',
        component: './Welcome',
      },
    ],
  },
  {
    name: 'list.table-list',
    icon: 'table',
    path: '/list',
    component: './TableList',
  },
  {
    name: 'users',
    icon: 'table',
    path: '/users',
    component: './TableList'
  },
  {
    name: 'roles',
    icon: 'table',
    path: '/roles',
    component: './TableList'
  },
  {
    name: 'apps',
    icon: 'table',
    path: '/apps',
    component: './TableList'
  },
  {
    name: 'envs',
    icon: 'table',
    path: '/envs',
    component: './TableList'
  },
  {
    name: 'files',
    icon: 'table',
    path: '/files',
    component: './TableList'
  },
  {
    path: '/',
    redirect: '/welcome',
  },
  {
    name: 'password',
    icon: 'table',
    path: '/password',
    hideInMenu: true,
    component: './TableList',
  },
  {
    component: './404',
  },
];
