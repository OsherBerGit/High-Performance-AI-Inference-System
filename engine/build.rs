fn main() {
    tonic_build::compile_protos("../shared_proto/computation.proto").unwrap();
}
