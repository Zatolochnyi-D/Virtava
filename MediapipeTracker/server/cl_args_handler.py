import argparse

class ClArgsHandler:
    def __init__(self):
        parser = argparse.ArgumentParser()
        parser.add_argument('port', help = 'Port on which server will broadcast tracking results.')
        parser.add_argument('model_asset_path', help = 'Path to face landmarker model.')
        args = parser.parse_args()
        self.port = args.port
        self.model_asset_path = args.model_asset_path