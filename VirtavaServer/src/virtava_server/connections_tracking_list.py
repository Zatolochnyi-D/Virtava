from time import time


class ConnectionsTrackingList:
    def __init__(self):
        self.__connections: dict[int, int] = {}
        self.__nextPropriateId = 0
        self.__count = 0

    @property
    def count(self):
        return self.__count
    
    def connection_exists(self, id: int):
        return id in self.__connections.keys()
    
    def create_connection(self) -> int:
        self.__connections[self.__nextPropriateId] = int(time())
        self.__nextPropriateId += 1
        self.__count += 1
        return self.__nextPropriateId - 1

    def remove_connection(self, id: int):
        if self.connection_exists(id):
            del self.__connections[id]
            self.__count -= 1
        else:
            raise Exception("Cannot remove id not present in the connections list")
        
    def update_connection_timestamp(self, id: int):
        if self.connection_exists(id):
            self.__connections[id] = int(time())
        else:
            raise Exception("Cannot update id not present in the connections list")
        
    def remove_timed_out_connections(self, timeout: int) -> int:
        timed_out_ids = []
        for key, value in self.__connections.items():
            if int(time()) - value > timeout:
                timed_out_ids.append(key)
        for id in timed_out_ids:
            self.remove_connection(id)
        return len(timed_out_ids)