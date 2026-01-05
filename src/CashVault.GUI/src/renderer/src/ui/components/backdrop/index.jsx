import styled from '@emotion/styled'

const BackdropContainer = styled.div`
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.3);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
  transition: opacity 0.3s ease-in-out;
`
const Backdrop = ({ children }) => {
  return <BackdropContainer>{children}</BackdropContainer>
}

export default Backdrop
