import React, { useContext, useEffect, useState } from 'react';
import { observer } from 'mobx-react';
import dayjs from 'dayjs';

import CortexLayout from '@layouts/CortexLayout';
import { AppStoreContext } from '@stores/AppStore';

const Home = () => {
  const appStore = useContext(AppStoreContext);
  const { beholderStore } = appStore;

  const [currentTime, setCurrentTime] = useState(dayjs());

  useEffect(() => {
    setTimeout(() => {
      setCurrentTime(dayjs());
    }, 250);
  });
  return (
    <CortexLayout>
      <div className="pt-4 pl-5 mr-4 mb-4 h-64 grid grid-rows-3 grid-flow-col gap-4">
        <div className="row-span-3 bg-base-200 rounded-md text-base-content text-2xl">
          <table className="table-auto w-full text-center">
            <thead>
              <tr>
                <th>Host</th>
                <th>Service</th>
                <th>Version</th>
                <th>Last Seen</th>
              </tr>
            </thead>
            <tbody>
              {beholderStore.serviceInfo.map((service) => (
                <tr key={service.key}>
                  <td>{service.hostName}</td>
                  <td>{service.serviceName}</td>
                  <td>{service.version}</td>
                  <td>{currentTime.diff(service.lastSeen, 'second')}s</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        <div className="col-span-2 bg-neutral-focus rounded-md flex justify-center items-center text-neutral-content text-2xl font-extrabold">
          <div>{currentTime.format('MM/DD/YYYY  h:mm:ss')}</div>
        </div>
        <div className="row-span-2 col-span-2 bg-base-200 rounded-md flex justify-center items-center text-base-contenttext-2xl font-extrabold">
          3
        </div>
      </div>
    </CortexLayout>
  );
};

export default observer(Home);
