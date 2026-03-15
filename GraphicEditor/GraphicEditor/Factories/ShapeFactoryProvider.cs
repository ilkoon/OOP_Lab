namespace GraphicEditor.Factories
{

    /// Provides the appropriate factory based on the selected mode

    public class ShapeFactoryProvider
    {
        private readonly Shape2DFactory _factory2D;
        private readonly Shape3DFactory _factory3D;
        private bool _is3DMode;

        public ShapeFactoryProvider()
        {
            _factory2D = new Shape2DFactory();
            _factory3D = new Shape3DFactory();
            _is3DMode = false;
        }

        public void SetMode(bool is3DMode)
        {
            _is3DMode = is3DMode;
        }

        public IShapeFactory GetCurrentFactory()
        {
            return _is3DMode ? (IShapeFactory)_factory3D : _factory2D;
        }

        public string[] GetAvailableShapes()
        {
            return _is3DMode ? _factory3D.GetAvailableShapes3D() : _factory2D.GetAvailableShapes();
        }
    }
}