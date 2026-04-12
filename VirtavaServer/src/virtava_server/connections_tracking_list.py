from time import time


class ConnectionsTrackingList:
    def __init__(self):
        self._connections: dict[int, int] = {}
        self._nextPropriateId = 0
        self._count = 0

    @property
    def count(self):
        return self._count
    
    def connection_exists(self, id: int):
        return id in self._connections.keys()
    
    def create_connection(self) -> int:
        self._connections[self._nextPropriateId] = int(time())
        self._nextPropriateId += 1
        self._count += 1
        return self._nextPropriateId - 1

    def remove_connection(self, id: int):
        if self.connection_exists(id):
            del self._connections[id]
            self._count -= 1
        else:
            raise Exception("Cannot remove id not present in the connections list")
        
    def update_connection_timestamp(self, id: int):
        if self.connection_exists(id):
            self._connections[id] = int(time())
        else:
            raise Exception("Cannot update id not present in the connections list")
        
    def remove_timed_out_connections(self, timeout: int) -> int:
        timed_out_ids = []
        for key, value in self._connections.items():
            if int(time()) - value > timeout:
                timed_out_ids.append(key)
        for id in timed_out_ids:
            self.remove_connection(id)
        return len(timed_out_ids)