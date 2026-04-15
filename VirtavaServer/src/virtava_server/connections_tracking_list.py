import logging
from time import time

class ConnectionsTrackingList:
    def __init__(self):
        self.__connections: dict[int, int] = {}
        self.__nextPropriateId = 0
        self.__count = 0
        self.__logger = logging.getLogger(__name__)
        self.__logger.debug("Initialized ConnectionTrackingList.")

    @property
    def count(self):
        return self.__count
    
    def connection_exists(self, id: int):
        return id in self.__connections.keys()
    
    def create_connection(self) -> int:
        self.__connections[self.__nextPropriateId] = int(time())
        self.__nextPropriateId += 1
        self.__count += 1
        self.__logger.debug("Added new connection to ConnectionTrackingList with id %i.", self.__nextPropriateId - 1)
        return self.__nextPropriateId - 1

    def remove_connection(self, id: int):
        if self.connection_exists(id):
            del self.__connections[id]
            self.__count -= 1
            self.__logger.debug("Removed connection with id %i.", id)
        else:
            self.__logger.debug("Tried to remove connection with id %i, but such connection was not present.", self.__nextPropriateId - 1)
            raise Exception("Cannot remove id not present in the connections list.")
        
    def update_connection_timestamp(self, id: int):
        if self.connection_exists(id):
            self.__connections[id] = int(time())
            self.__logger.debug("Updated connection's with id %i timestamp.", id)
        else:
            self.__logger.debug("Tried to updated connection's with id %i timestamp, but such connection was not present.", id)
            raise Exception("Cannot update id not present in the connections list")
        
    def remove_timed_out_connections(self, timeout: int) -> int:
        timed_out_ids = []
        for key, value in self.__connections.items():
            if int(time()) - value > timeout:
                timed_out_ids.append(key)
        for id in timed_out_ids:
            self.remove_connection(id)
        self.__logger.debug("Performed removal of timed out connections.")
        if timed_out_ids:
            self.__logger.debug("Following ids were removed: %s.", ', '.join(timed_out_ids))
        return len(timed_out_ids)