script_path=$(cd $(dirname $0) && pwd)
message_name=ping.proto
cd $script_path
source ".venv/bin/activate"
python -m grpc_tools.protoc --proto_path=../ --python_out=src/virtava_server/ --pyi_out=src/virtava_server/ "../$message_name"
deactivate