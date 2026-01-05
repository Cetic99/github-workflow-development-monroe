/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { sizeMap } from './size-map'
import { IconContainer } from './styles'

const IconArrowUp = ({ color = 'black', size = 'm' }) => {
  return (
    <IconContainer color={color} w={sizeMap[size].w} h={sizeMap[size].h}>
      <svg
        width="24"
        height="24"
        viewBox="0 0 24 24"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M12 2C12.5523 2 13 2.47128 13 3.05263V20.9474C13 21.5287 12.5523 22 12 22C11.4477 22 11 21.5287 11 20.9474V3.05263C11 2.47128 11.4477 2 12 2Z"
          fill="currentColor"
        />
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M11.2929 2.29289C11.6834 1.90237 12.3166 1.90237 12.7071 2.29289L17.7071 7.29289C18.0976 7.68342 18.0976 8.31658 17.7071 8.70711C17.3166 9.09763 16.6834 9.09763 16.2929 8.70711L12 4.41421L7.70711 8.70711C7.31658 9.09763 6.68342 9.09763 6.29289 8.70711C5.90237 8.31658 5.90237 7.68342 6.29289 7.29289L11.2929 2.29289Z"
          fill="currentColor"
        />
      </svg>
    </IconContainer>
  )
}

export default IconArrowUp
