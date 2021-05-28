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
    name: 'users',
    icon: 'table',
    path: '/users',
    component: './user/list',
  },
  {
    name: 'userDetail',
    icon: 'table',
    path: '/users/detail/:id',
    hideInMenu: true,
    component: './user/detail',
  },
  {
    name: 'roles',
    icon: 'table',
    path: '/roles',
    component: './role',
  },
  {
    name: 'apps',
    icon: 'table',
    path: '/apps',
    component: './app',
  },
  {
    name: 'envs',
    icon: 'table',
    path: '/envs',
    component: './env',
  },
  {
    name: 'files',
    icon: 'table',
    path: '/files',
    component: './file',
  },
  {
    name: 'files-detail',
    icon: 'table',
    path: '/files/detail/:app/:env',
    hideInMenu: true,
    component: './file/detail',
  },
  {
    name: 'logs',
    icon: 'table',
    path: '/logs',
    component: './log',
  },
  {
    name: 'logs-detail',
    icon: 'table',
    path: '/logs/detail/:app/:env/:logId',
    hideInMenu: true,
    component: './log/detail',
  },
  {
    name: 'logs-byfileid',
    icon: 'table',
    path: '/logsbyfileid/:app/:env/:fileId',
    hideInMenu: true,
    component: './log/byFileId',
  },
  {
    name: 'resources',
    icon: 'table',
    path: '/resources',
    component: './resource',
  },
  {
    name: 'secrets',
    icon: 'table',
    path: '/secrets',
    hideInMenu: true,
    component: './secret',
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
    component: './user/password',
  },
  {
    component: './404',
  },
];
