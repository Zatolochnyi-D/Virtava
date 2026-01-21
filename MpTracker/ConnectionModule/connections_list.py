from ConnectionModule.connection import Connection

class ConnectionsList:
    def __init__(self):
        self._connections: list[Connection] = []
        self.count = 0

    def add(self, connection: Connection):
        index = 0
        for i in range(len(self._connections)):
            if self._connections[i] is None:
                self._connections[i] = connection
                index = i
                break
        else:
            self._connections.append(connection)
            index = len(self._connections) - 1
        self.count += 1
        self._connections[index].set_on_close(lambda: self._remove_from_list(index))

    def _remove_from_list(self, index: int):
        self.count -= 1
        self._connections[index] = None

    def close(self, index: int):
        self._connections[index].close(False)
        self._remove_from_list(index)

    def close_all(self):
        for i, connection in enumerate(self._connections):
            if connection is not None:
                connection.close(False)
        self._connections = []

    def __iter__(self):
        return ConnectionsListIterator(self)

class ConnectionsListIterator:
    def __init__(self, list: ConnectionsList):
        self._target = iter(list._connections)
        pass

    def __next__(self):
        value = None
        while value is None:
            value = next(self._target)
        return value