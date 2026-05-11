script_path=$(cd $(dirname $0) && pwd)
message_name=tracking_results.proto
cd $script_path
source ".venv/bin/activate"
python -m grpc_tools.protoc --proto_path=../VirtavaArkitBlendshapes/ --python_out=src --pyi_out=src "../VirtavaArkitBlendshapes/$message_name"
deactivate